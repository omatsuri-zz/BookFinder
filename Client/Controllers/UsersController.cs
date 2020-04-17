using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class UsersController : Controller
    {
        readonly HttpClient Client = new HttpClient();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            Client.BaseAddress = new Uri("https://localhost:44342/api/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            var id = HttpContext.Session.GetString("Id");
            if (id == null)
            {
                return View();
            }
            return RedirectToAction(nameof(Dashboard));
        }
        public ActionResult Dashboard()
        {
            var id = HttpContext.Session.GetString("Id");
            if (id != null)
            {
                return View();
            }
            return RedirectToAction(nameof(Login));
        }
        public ActionResult Register()
        {
            var a = HttpContext.Session.GetString("Id");
            var b = HttpContext.Session.GetString("Email");
            if (a != null && b != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserVM loginVM)
        {
            try
            {
                var login = await _userManager.FindByEmailAsync(loginVM.UserName);
                loginVM.UserName = login.UserName;
                var hasil = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (hasil.Succeeded)
                {
                    // TODO: Add insert logic here
                    var myContent = JsonConvert.SerializeObject(loginVM);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = Client.PostAsync("Users/Login/", byteContent).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var user = result.Content.ReadAsAsync<TokenVM>().Result;
                        HttpContext.Session.SetString("Id", user.Id.ToString());
                        HttpContext.Session.SetString("Token", "Bearer " + user.AccessToken);
                        HttpContext.Session.SetString("Expire", user.ExpireToken.ToString());
                        HttpContext.Session.SetString("ExpireRefreshToken", user.ExpireRefreshToken.ToString());
                        HttpContext.Session.SetString("RefreshToken", user.RefreshToken);
                        HttpContext.Session.SetString("UserName", user.Username);
                        Client.DefaultRequestHeaders.Add("Authorization", user.AccessToken);
                        return RedirectToAction(nameof(Dashboard));
                    }
                }
                return View();
            }
            catch (Exception e)
            {
                return View();
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Register(UserVM userVM)
        {
            var myContent = JsonConvert.SerializeObject(userVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = Client.PostAsync("users/register/", byteContent).Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            return View();
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
        [HttpGet]
        public ActionResult Logout()
        {
            Client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
            var result = Client.GetAsync("users/Logout").Result;
            if (result.IsSuccessStatusCode)
            {
                HttpContext.Session.Remove("Id");
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("Token");
                HttpContext.Session.Remove("Expire");
                HttpContext.Session.Remove("ExpireRefreshToken");
                HttpContext.Session.Remove("RefreshToken");


                HttpContext.Session.Clear();
                return RedirectToAction("Main", "Home", new { area = "" });
            }
            return View();
        }
        
    }
}