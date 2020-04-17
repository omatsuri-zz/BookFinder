using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data.Model;
using Data.ViewModel;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    //[Authorize(Policy = "RequireAdminRole")]
    public class BookFindersController : Controller
    {
        readonly HttpClient Client = new HttpClient();
        public BookFindersController()
        {
            Client.BaseAddress = new Uri("https://localhost:44342/api/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Index()
        {
            return View();
        }
        //yang ga pakai button langsung bentuk half form 
        public IActionResult FindBook()
        {
            return View();
        }
        //halaman cari yng form nya dibody, tablenya ntar terpisah sama yang halaman ini
        public IActionResult Search()
        {

            return View();

        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public IActionResult Search(BookFinderVM bookFinderVM)
        {
                var title = bookFinderVM.Title;
                var author = bookFinderVM.Author;
                var publisher = bookFinderVM.Publisher;
                var isbn = bookFinderVM.ISBN;
                var dataPage = caribuku(title, author, publisher, isbn).Result;
                return View();
        }
        public JsonResult GetbyID(int Id)
        {
            IEnumerable<BookFinder> bookFinder = null;
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var responseTask = Client.GetAsync("bookFinders/" + Id);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IEnumerable<BookFinder>>();
                readTask.Wait();
                bookFinder = readTask.Result;
            }
            else
            {
                bookFinder = Enumerable.Empty<BookFinder>();
                ModelState.AddModelError(string.Empty, "Server error try after some time");
            }
            return Json(bookFinder);
        }
        [HttpPost]
        public IActionResult Insert(IFormFile file, BookFinderVM bookFinderVM)
        {
            var path = "C:\\hasil_upload\\";
            var filePath = Path.Combine(path, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);
            }
            bookFinderVM.URL = filePath;
            var myContent = JsonConvert.SerializeObject(bookFinderVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var result = Client.PostAsync("bookFinders", byteContent).Result;
            return RedirectToAction("Dashboard", "Users", new { area = "" });
        }
        public JsonResult Update(int id, BookFinderVM bookFinderVM)
        {
            var myContent = JsonConvert.SerializeObject(bookFinderVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var result = Client.PutAsync("bookFinders/" + id, byteContent).Result;
            return Json(result);

        }
        public JsonResult Delete(int id)
        {
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var result = Client.DeleteAsync("bookFinders/" + id).Result;
            return Json(result);
        }

        public async Task<BookFinderVM> Paging(int pageSize, int pageNumber, string keyword)
        {
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));

            TokenVM tokenVM = new TokenVM();

            var exptoken = Convert.ToInt64(HttpContext.Session.GetString("Expire"));
            var expreftoken = Convert.ToInt64(HttpContext.Session.GetString("ExpireRefreshToken"));

            if (exptoken < DateTime.UtcNow.Ticks && expreftoken > DateTime.UtcNow.Ticks)
            {
                await RefToken(tokenVM);
            }
            else if (expreftoken < DateTime.UtcNow.Ticks)
            {
                return null;
            }
            try
            {
                var response = await Client.GetAsync("BookFinders/paging?keyword=" + keyword + "&pageSize=" + pageSize + "&pageNumber=" + pageNumber);
                var testing = response;
                if (response.IsSuccessStatusCode)
                {
                    var e = await response.Content.ReadAsAsync<BookFinderVM>();
                    return e;
                }
            }
            catch (Exception m)
            {

            }
            return null;
        }
        //[HttpGet("BookFinder/PageData")]
        public IActionResult PageData(IDataTablesRequest request)
        {
            var pageSize = request.Length;
            var pageNumber = request.Start / request.Length + 1;
            var keyword = request.Search.Value;
            var dataPage = Paging(pageSize, pageNumber, keyword).Result;
            var response = DataTablesResponse.Create(request, dataPage.length, dataPage.filterLength, dataPage.data);

            return new DataTablesJsonResult(response, true);
        }
        public async Task RefToken(TokenVM tokenVM)
        {
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var myContent = JsonConvert.SerializeObject(tokenVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = Client.PostAsync("Users/refresh", byteContent).Result;

            if (result.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                var tes = await result.Content.ReadAsAsync<TokenVM>();
                var token2 = tes.AccessToken;
                var exptoken2 = tes.ExpireToken;
                var reftoken2 = tes.RefreshToken;
                var expreftoken2 = tes.ExpireRefreshToken;
                HttpContext.Session.SetString("Token", token2);
                HttpContext.Session.SetString("Expire", exptoken2.ToString());
                HttpContext.Session.SetString("ExpireRefreshToken", expreftoken2.ToString());
                HttpContext.Session.SetString("RefreshToken", reftoken2);
            }
        }
        public async Task<List<BookFinderVM>> GetBookFinder()
        {
            List<BookFinderVM> barang = new List<BookFinderVM>();

            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));

            var responseTask = await Client.GetAsync("BookFinders");
            barang = await responseTask.Content.ReadAsAsync<List<BookFinderVM>>();
            return barang;
        }

        [HttpGet("BookFinders/FindBook/{title}/{author}/{publisher}/{isbn}")]
        //Search BOOK 
        public IActionResult FindBook(IDataTablesRequest request, string title, string author, string publisher, long isbn)
        {
            var pageSize = request.Length;
            var pageNumber = request.Start / request.Length + 1;
            var keyword = request.Search.Value;
            var dataPage = PagingBook(title, author, publisher, isbn, pageSize, pageNumber, keyword).Result;
            var response = DataTablesResponse.Create(request, dataPage.length, dataPage.filterLength, dataPage.data);

            return new DataTablesJsonResult(response, true);
        }
        #region aneh
        //public IActionResult FindBook(IDataTablesRequest request, BookFinderVM bookFinderVM)
        //{
        //    var pageSize = request.Length;
        //    var pageNumber = request.Start / request.Length + 1;
        //    var keyword = request.Search.Value;
        //    var title = bookFinderVM.Title;
        //    var author = bookFinderVM.Author;
        //    var publisher = bookFinderVM.Publisher;
        //    var isbn = bookFinderVM.ISBN;
        //    var dataPage = PagingBook(title,author,publisher,isbn, pageSize, pageNumber, keyword).Result;
        //    var response = DataTablesResponse.Create(request, dataPage.length, dataPage.filterLength, dataPage.data);

        //    return new DataTablesJsonResult(response, true);
        //}
        #endregion
        public async Task<BookFinderVM> PagingBook(string title, string author, string publisher, long isbn, int pageSize, int pageNumber, string keyword)
        {
            try
            {
                var response = await Client.GetAsync("BookFinders/cari?Title=" + title + "&author=" + author + "&publisher=" + publisher + "&ISBN=" + isbn + "keyword=" + keyword + "&pageSize=" + pageSize + "&pageNumber=" + pageNumber);
                var testing = response;
                if (response.IsSuccessStatusCode)
                {
                    var e = await response.Content.ReadAsAsync<BookFinderVM>();
                    return e;
                }
            }
            catch (Exception m)
            {

            }
            return null;
        }
        public async Task<BookFinderVM> caribuku(string title, string author, string publisher, long isbn)
        {
            try
            {
                var response = await Client.GetAsync("BookFinders/cari?Title=" + title + "&author=" + author + "&publisher=" + publisher + "&ISBN=" + isbn + "keyword="+ "&pageSize=10" + "&pageNumber=1");
                var testing = response;
                if (response.IsSuccessStatusCode)
                {
                    var e = await response.Content.ReadAsAsync<BookFinderVM>();
                    return e;
                }
            }
            catch (Exception m)
            {

            }
            return null;
        }
    }
}