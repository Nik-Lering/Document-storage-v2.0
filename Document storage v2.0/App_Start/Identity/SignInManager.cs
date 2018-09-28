using Document_storage_v2.Models;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Document_storage_v2.Auth
{
    public class SignInManager :
        SignInManager<Users, long>
    {
        public SignInManager(UserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager) { }

        public void SignOut()
            => AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
            => new SignInManager(context.GetUserManager<UserManager>(), context.Authentication);

    }
}