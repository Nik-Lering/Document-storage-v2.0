using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

using Document_storage_v2.Models;
using Document_storage_v2.Auth;

namespace Document_storage_v2._0.Controllers
{
    public class ClientController : BaseController
    {
        readonly IRepository<Users> usersRepo;

        public ClientController(ISession session) :
             base(session)
        {
            usersRepo = CreateRepo<Users>();
        }

        #region [Login]

        // GET: /Client/Login
        public ActionResult Login(string returnUrl)
            => View(returnUrl: returnUrl, title: "Выполнить вход");

        // POST: /Client/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserLogin, model.UserPassword, model.RememberMe, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        return returnUrl == null ?
                            RedirectToAction("Documents", "Documents") :
                            RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("ConfirmEmail", new { returnUrl, model.RememberMe });

                    default:
                        ModelState.AddModelError("", "Не удалость произвести вход с указанными данными.");
                        break;
                };
            }

            return View("ModalLoginPartial", model);
        }

        #endregion

        public string GenerationPassword(int x)
        {
            string pass = "";
            var r = new Random();
            while (pass.Length < x)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }

        public ActionResult restorePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> restorePassword(Users model)
        {
            if (ModelState.IsValid)
            {
                Users user = usersRepo.CreateCriteria()
                    .Add(Restrictions.Eq("UserLogin", model.UserLogin))
                    .Add(Restrictions.Eq("Email", model.Email))
                    .UniqueResult<Users>();

                if (user.Id != 0)
                {
                    string Password = GenerationPassword(13);
                    await UserManager.SendEmailAsync(user.Id, "Новый пароль", $"Для выхода используйте новый пароль: { Password }");
                    user.UserPassword = UserManager.PasswordHasher.HashPassword(Password);
                    usersRepo.Transaction(x => x.Update(user));
                }
            }
            
            return View();
        }

        #region [Register]

        // GET: /Client/Register
        public ActionResult Register()
        {
            return View();
        }
            

        // POST: /Client/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = new Users { UserName = model.UserName, Email = model.Email, UserLogin = model.UserLogin };
                var result = await UserManager.CreateAsync(users, model.UserPassword);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(users, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            return View(model);
        }

        #endregion

        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}
