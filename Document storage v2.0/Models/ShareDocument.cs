using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Document_storage_v2.Models
{
    public class ShareDocument :
        IEntity
    {
        public virtual long Id { get; set; }

        public virtual long ShareDocId { get; set; }

        public virtual long ShareClientId { get; set; }

        public virtual long ClientId { get; set; }

    }
}

namespace Document_storage_v2.Models.Mapping
{
    using FluentNHibernate.Mapping;

    public class ShareDocumentMap : ClassMap<ShareDocument>
    {
        public ShareDocumentMap()
        {
            Id(u => u.Id);
            Map(s => s.ShareDocId).Column("ShareDocId").Length(100);
            Map(s => s.ShareClientId).Column("ShareClientId").Length(100);
            Map(s => s.ClientId).Column("ClientId").Length(100);

        }
    }
}
