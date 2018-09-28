using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Document_storage_v2.Auth;
using Document_storage_v2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using NHibernate;

namespace Document_storage_v2._0.Controllers
{
    public abstract class BaseController :
        Controller
    {
        #region [Свойства]

        protected virtual IOwinContext OwinContext
            => HttpContext.GetOwinContext();

        protected IAuthenticationManager AuthenticationManager
            => OwinContext.Authentication;

        protected virtual SignInManager SignInManager
            => signInManager ?? UpdateSignInManager();

        protected virtual UserManager UserManager
            => userManager ?? UpdateUserManager();

        protected virtual Users CurrentUser
            => currentUser ?? UpdateCurrentUser();

        #endregion

        #region [Поля]

        protected IRepository<Users> userRepo;
        protected ISession session;

        private UserManager userManager;
        private SignInManager signInManager;
        private Users currentUser;


        #region [Обновление полей]

        protected virtual UserManager UpdateUserManager()
           => (userManager = OwinContext?.GetUserManager<UserManager>());

        protected virtual SignInManager UpdateSignInManager()
            => (signInManager = OwinContext?.Get<SignInManager>());

        protected virtual Users UpdateCurrentUser()
            => (currentUser = userRepo[User?.Identity?.GetUserId<long>()]);

        #endregion

        #endregion

        #region [Методы]

        /// <summary>
        /// Производит перенаправление по указанному локальному URL
        /// </summary>
        /// <param name="returnUrl">URL-строка для перенаправления</param>
        /// <returns></returns>
        protected virtual ActionResult RedirectToLocal(string returnUrl)
              => Url.IsLocalUrl(returnUrl) ? (ActionResult)
                  base.Redirect(returnUrl) :
                  base.RedirectToAction("Index", "Home");

        /// <summary>
        /// Cоздает репозиторий объектов указанного типа
        /// </summary>
        /// <typeparam name="T">Тип объектов репозитория</typeparam>
        /// <returns></returns>
        protected virtual IRepository<T> CreateRepo<T>()
            where T : class, IEntity, new()
            => new Repository<T>(session);

        /// <summary>
        /// Добавляет все сообщения об ошибках в спсок ошибок валидации
        /// </summary>
        protected void AddErrors(IdentityResult result)
            => result?.Errors.All(e =>
            {
                ModelState.AddModelError("", e);
                return true;
            });

        /// <summary>
        /// Возвращает тот же объект View, если модель не прошла проверку
        /// </summary>
        /// <param name="model">Модель для отображения</param>
        protected ActionResult InvalidModel(object model)
           => !ModelState.IsValid ? View(model) : default;

        protected virtual ActionResult RedirectToAction(string actionName, string controllerName = default, Action preAction = default, object routeValues = default)
        {
            preAction?.Invoke();
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }

        /// <summary>
        /// Возвращает объект View с заданными параметрами
        /// </summary>
        /// <param name="name">Название действия</param>
        /// <param name="controller">Контроллер</param>
        /// <param name="model">Модель для отображения</param>
        /// <param name="message">Выводимое сообщение</param>
        /// <param name="title">Заголовок страницы</param>
        /// <param name="returnUrl">Адрес возврата</param>
        /// <returns></returns>
        protected virtual ActionResult View(string name = default, string controller = default, object model = default,
            // параметры ViewBag
            string message = default, string title = default, string returnUrl = default,
            // выполняемое действие
            Action preAction = default)
        {
            preAction?.Invoke();

            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.ReturnUrl = returnUrl;

            return string.IsNullOrEmpty(controller) ?
                View(name, model) : base.View(name, controller, model);
        }

        #region [IDisposable]

        /// <summary>
        /// Утлизирует ресурсы объекта и удаляет ссылку на него
        /// </summary>
        /// <typeparam name="T">Тип утилизируемого объекта</typeparam>
        /// <param name="obj">Объект для утилизации</param>
        protected virtual void DisposeObject<T>(ref T obj)
            where T : class, IDisposable
        {
            if (obj != default(T))
            {
                obj.Dispose();
                obj = default;
            }
        }

        /// <summary>
        /// Утилизирует ресурсы контроллера
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeObject(ref userManager);
                DisposeObject(ref signInManager);
            }

            base.Dispose(disposing);
        }


        #endregion

        #endregion

        protected BaseController(ISession nhSession) :
            base()
        {
            session = nhSession;
            userRepo = CreateRepo<Users>();

            ViewBag.App = "Doc Storage";
        }
    }
}