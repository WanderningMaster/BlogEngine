using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogEngine.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BlogEngine.Controllers
{
    public class HomeController : Controller
    {
        EngineContext db;
        string Test = "";

        public HomeController(EngineContext context)         
        {
            db = context;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("IsAuthorize", "false");
            if (HttpContext.Session.GetString("IsAuthorize") == "true")
                return View("BlogsPage");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(string Login, string Password)
        {
            string message = "";
            var user = db.Accounts.FirstOrDefault(x => x.login == Login);
            //var user = db.Accounts.Where(x => x.login == Login).Select(x => x); 
            if(user.password == Password)
            {
                TempData["UserName"] = Login;
                HttpContext.Session.SetString("Username", user.login.ToString());
                HttpContext.Session.SetString("IsAuthorize", "true");
                return View("BlogsPage");
            }
            message = "Access denied";                   
            ViewBag.Message = message;            
            return View("Message");
        }

        [HttpPost]
        public IActionResult Register(string Login, string Password, string ConfirmPassword)
        {
            string message = "";
            if (Password == ConfirmPassword)
            {
                if (db.Accounts.Where(x => x.login == Login).ToList().Count == 0)
                {
                    db.Accounts.Add(new Account
                    {
                        login = Login,
                        password = Password
                    });
                    db.SaveChanges();                 
                    message = "Registration completed successfully";                    
                }
                else
                {
                    message = "Login is already in using";
                } 
            }
            else
                message = "Password confirming failed";
            ViewBag.Message = message;
            return View("Message");
        }

        [HttpPost]
        public IActionResult Add(string Title, string Content)
        {
            if(HttpContext.Session.GetString("IsAuthorize") == "true")
            {
                ViewBag.Title = Title;
                ViewBag.Content = Content;
                ViewBag.Message = "Post has been added";
                db.Posts.Add(new Post
                {
                    Title = Title,
                    UserName = (string)TempData["UserName"],
                    Content = Content,
                    PostTime = DateTime.Now
                });
                db.SaveChanges();
                return View("Message");
            }
            ViewBag.Message = "Access denied";
            return View("Message");
        }

        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsAuthorize") == "true")          
                return View();
            ViewBag.Message = "Access denied";
            return View("Message");
        }
        public IActionResult BlogsPage()
        {
            if (HttpContext.Session.GetString("IsAuthorize") == "true")
                return View();
            ViewBag.Message = "Access denied";
            return View("Message");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString("IsAuthorize", "false");
            return View("Index");
        }
        
        public IActionResult BlogWall()
        {
            return View(db.Posts.ToList());
        }

        [HttpGet]
        public IActionResult PostPage(int? Id)
        {
            var post = db.Posts.FirstOrDefault(x => x.Id == Id);
            ViewBag.Author = post.UserName;
            ViewBag.Title = post.Title;
            ViewBag.PostTime = post.PostTime;
            ViewBag.Content = post.Content;

            return View();
        }

        public IActionResult DeleteWall()
        {
            if (HttpContext.Session.GetString("IsAuthorize") == "true")
            {
                ViewBag.Username = HttpContext.Session.GetString("Username");
                return View(db.Posts.ToList());
            }
                
            ViewBag.Message = "Access denied";
            return View("Message");
        }

        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (HttpContext.Session.GetString("IsAuthorize") == "true")
            {
                var curPost = db.Posts.FirstOrDefault(x => x.Id == Id);
                db.Posts.Remove(curPost);
                db.SaveChanges();

                ViewBag.Message = "Post successfully deleted";
                return View("Message");
            }
            ViewBag.Message = "Access denied";
            return View("Message");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
