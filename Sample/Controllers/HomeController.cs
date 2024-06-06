using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Data;
using Sample.Models;
using System.Diagnostics;

namespace Sample.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("UsersId");

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                AppUser model = dbContext.Users.FirstOrDefault(a => a.Id == user);

                return View(model);

            }
            
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
