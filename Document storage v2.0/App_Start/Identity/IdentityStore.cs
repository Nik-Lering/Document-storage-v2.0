using Document_storage_v2.Models;
using Microsoft.AspNet.Identity;

using NHibernate;

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Document_storage_v2.Auth
{
    public class IdentityStore :
        IUserPasswordStore<Users, long>,
        IUserEmailStore<Users, long>,
        IUserTwoFactorStore<Users, long>,
        IUserLoginStore<Users, long>,
        IUserLockoutStore<Users, long>
    {
        private readonly ISession session;

        public IdentityStore(ISession session)
        {
            this.session = session;
        }

        public Task CreateAsync(Users users)
    => Task.Run(() => session.SaveOrUpdate(users));

        public Task DeleteAsync(Users users)
            => Task.Run(() => session.Delete(users));

        public Task<Users> FindByIdAsync(long Id)
            => Task.Run(() => session.Get<Users>(Id));

        public Task<Users> FindByNameAsync(string UserLogin)
            => Task.Run(() =>
            {
                return session.QueryOver<Users>()
                    .Where(u => u.UserLogin == UserLogin)
                    .SingleOrDefault();
            });

        public Task UpdateAsync(Users users)
            => Task.Run(() => session.SaveOrUpdate(users));

        public Task SetPasswordHashAsync(Users users, string UserPassword)
            => Task.Run(() => users.UserPassword = UserPassword);

        public Task<string> GetPasswordHashAsync(Users users)
            => Task.FromResult(users.UserPassword);

        public Task<bool> HasPasswordAsync(Users users)
            => Task.FromResult(true);


        #region IUserLockoutStore<Users, int>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(Users user)
            => Task.FromResult(DateTimeOffset.MaxValue);

        public Task SetLockoutEndDateAsync(Users user, DateTimeOffset lockoutEnd)
            => Task.CompletedTask;

        public Task<int> IncrementAccessFailedCountAsync(Users user)
            => Task.FromResult(0);

        public Task ResetAccessFailedCountAsync(Users user)
            => Task.CompletedTask;

        public Task<int> GetAccessFailedCountAsync(Users user)
            => Task.FromResult(0);

        public Task<bool> GetLockoutEnabledAsync(Users user)
            => Task.FromResult(false);

        public Task SetLockoutEnabledAsync(Users user, bool enabled)
            => Task.CompletedTask;
        #endregion


        #region IUserTwoFactorStore<Users, int>
        public Task SetTwoFactorEnabledAsync(Users user, bool enabled)
            => Task.CompletedTask;

        public Task<bool> GetTwoFactorEnabledAsync(Users user)
            => Task.FromResult(false);

        #endregion

        public virtual void Dispose() { }

        public Task SetEmailAsync(Users users, string Email)
            => Task.CompletedTask;

        public Task<string> GetEmailAsync(Users users)
            => Task.FromResult(users.Email);

        public Task<bool> GetEmailConfirmedAsync(Users users)
            => Task.FromResult(true);

        public Task SetEmailConfirmedAsync(Users users, bool confirmed)
            => Task.CompletedTask;

        public Task<Users> FindByEmailAsync(string email)
            => Task.FromResult(session.QueryOver<Users>()
                    .Where(u => u.Email == email)
                    .SingleOrDefault());

        public Task AddLoginAsync(Users users, UserLoginInfo login)
            => Task.CompletedTask;

        public Task RemoveLoginAsync(Users users, UserLoginInfo login)
            => Task.CompletedTask;

        public Task<IList<UserLoginInfo>> GetLoginsAsync(Users users)
        => Task.FromResult<IList<UserLoginInfo>>(default);

        public Task<Users> FindAsync(UserLoginInfo login)
            => Task.FromResult(session.QueryOver<Users>()
                    .Where(u => u.UserName == login.ProviderKey)
                    .SingleOrDefault());
    }

}