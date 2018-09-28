using System;

using System.Linq;

using System.Web.Mvc;
using Document_storage_v2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using NHibernate;

namespace Document_storage_v2.Auth
{
    public class UserManager :
        UserManager<Users, long>
    {
        public UserManager(IUserStore<Users, long> store)
            : base(store) { }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(new IdentityStore(DependencyResolver.Current.GetServices<ISession>().FirstOrDefault()));

            // Настройка логики проверки имен пользователей
            manager.UserValidator = new UserValidator<Users, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Настройка логики проверки паролей
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Настройка параметров блокировки по умолчанию
            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtector = options.DataProtectionProvider?.Create("ASP.NET Identity");

            if (dataProtector != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<Users, long>(dataProtector);
            }

            return manager;
        }
    }
}