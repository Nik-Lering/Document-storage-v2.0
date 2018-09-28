using System;
using Document_storage_v2.Auth;
using Document_storage_v2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;


namespace Document_storage_v2
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app
                .CreatePerOwinContext<UserManager>(UserManager.Create)
                .CreatePerOwinContext<SignInManager>(SignInManager.Create)

                // Включение использования файла cookie, в котором приложение может хранить информацию для пользователя, выполнившего вход,
                // и использование файла cookie для временного хранения информации о входах пользователя с помощью стороннего поставщика входа
                // Настройка файла cookie для входа
                .UseCookieAuthentication(
                    new CookieAuthenticationOptions
                    {
                        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                        LoginPath = new PathString("/Client/Login"),
                        Provider = new CookieAuthenticationProvider
                        {
                            // Позволяет приложению проверять метку безопасности при входе пользователя.
                            // Эта функция безопасности используется, когда вы меняете пароль или добавляете внешнее имя входа в свою учетную запись.  
                            OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, Users, long>(
                                validateInterval: TimeSpan.FromMinutes(30),
                                regenerateIdentityCallback: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie),
                                getUserIdCallback: (user) => user.GetUserId<long>())
                        }
                    })

                // позволяет использовать вход со сторонних сервисов
                .UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // основные сторонние поставщики данных аутентификации
            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}