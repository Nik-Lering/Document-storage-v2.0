using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Document_storage_v2.Models
{
    public class Folder :
        IEntity
    {
        public virtual long Id { get; set; }

        public virtual string FolderName { get; set; }

        public virtual long FolderParent { get; set; }

        public virtual long ClientId { get; set; }

    }
}

namespace Document_storage_v2.Models.Mapping
{
    using FluentNHibernate.Mapping;

    public class FolderMap : ClassMap<Folder>
    {
        public FolderMap()
        {
            Id(f => f.Id);
            Map(f => f.FolderName).Column("FolderName").Length(100);
            Map(f => f.FolderParent).Column("FolderParent").Length(100);
            Map(f => f.ClientId).Column("ClientId").Length(100);

        }
    }
}
