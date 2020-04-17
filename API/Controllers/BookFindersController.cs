using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services.Interface;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookFindersController : ControllerBase
    {
        private IBookFinderServices _bookFinderServices;
        public BookFindersController(IBookFinderServices bookFinderServices)
        {
            _bookFinderServices = bookFinderServices;
        }
        [HttpGet]
        public async Task<IEnumerable<BookFinderVM>> Get()
        {
            return await _bookFinderServices.GetBooks();
        }
        [HttpGet("{id}")]
        public async Task<IEnumerable<BookFinder>> Get(int Id)
        {
            return await _bookFinderServices.Get(Id);
        }
        [HttpPost]

        public ActionResult Post(BookFinderVM bookFinderVM)
        {
            return Ok(_bookFinderServices.Create(bookFinderVM));
        }

        [HttpPut("{Id}")]
        public ActionResult Put(int Id, BookFinderVM bookFinderVM)
        {
            return Ok(_bookFinderServices.Update(Id, bookFinderVM));
        }


        [HttpDelete("{Id}")]
        public ActionResult Delete(int Id)
        {
            return Ok(_bookFinderServices.Delete(Id));
        }
        [HttpGet]
        [Route("Search/{keyword}")]
        public IEnumerable<BookFinderVM> Search( string keyword)
        {
            keyword = keyword.Substring(3);
            return _bookFinderServices.Search( keyword);
        }
        [HttpGet]
        [Route("Paging")]
        public async Task<BookFinderVM> Paging( string keyword, int pageNumber, int pageSize)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            return await _bookFinderServices.Paging(keyword, pageNumber, pageSize);
        }
        [HttpGet]
        [Route("cari")]
        public async Task<BookFinderVM> cari(string Title, string Author, string Publisher, long ISBN, string keyword, int pageNumber, int pageSize)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            else if (Title == null)
            {
                Title = "";
            }
            else if (Author == null)
            {
                Author = "";
            }
            else if (Publisher == null)
            {
                Publisher = "";
            }
            else if (ISBN == 0)
            {
                ISBN = 0;
            }
            return await _bookFinderServices.cari(Title, Author, Publisher, ISBN,keyword, pageNumber, pageSize);
        }
    }
}