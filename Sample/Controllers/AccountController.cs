using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sample.Data;
using Sample.Models;
using Sample.Models.ViewModel;
using System.Runtime.ConstrainedExecution;

namespace Sample.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext db;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext db)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm model)
        {

            if (ModelState.IsValid) {

                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {

                    var user = db.Users.FirstOrDefault(a => a.UserName == model.Username);

                    if (user != null)
                    {
                         HttpContext.Session.SetString("UsersId", user.Id);
                         user.Online = true;

                        db.Users.Update(user);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Wrong Credintials!");
                return View(model);
            }

            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm model)
        {

            if(ModelState.IsValid)
            {

                AppUser user = new()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };

                var res = await userManager.CreateAsync(user, model.Password!);

                if (res.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                   
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {

            await signInManager.SignOutAsync();

            Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("Expires", "0");

            var user = HttpContext.Session.GetString("UsersId");

            var users = db.Users.FirstOrDefault(a => a.Id == user);

            if (users != null)
            {
                users.Online = false;
                db.Users.Update(users);
                db.SaveChanges();
            }

            HttpContext.Session.Remove("UsersId");

            return RedirectToAction("Login", "Account");

        }

        public IActionResult UserStatusAction()
        {

            var user = HttpContext.Session.GetString("UsersId");
             
            return Ok(user);
        }

    }
}
