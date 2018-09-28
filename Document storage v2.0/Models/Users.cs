
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Document_storage_v2.Models
{
    public class Users :
        IEntity, IUser<long>
    {
        public virtual long Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string UserLogin { get; set; }

        public virtual string UserPassword { get; set; }

        public virtual bool RememberMe { get; set; }

        public virtual string ConfirmPassword { get; set; }

        public virtual string Email { get; set; }

    }
}

namespace Document_storage_v2.Models.Mapping
{
    using FluentNHibernate.Mapping;

    public class UserMap : ClassMap<Users>
    {
        public UserMap()
        {
            Id(u => u.Id);
            Map(u => u.UserLogin).Column("UserLogin").Length(100);
            Map(u => u.UserPassword).Column("UserPassword").Length(100);
            Map(u => u.UserName).Column("UserName").Length(100);
            Map(u => u.Email).Column("Email").Length(100);
        }
    }
}