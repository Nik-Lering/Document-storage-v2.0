using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Document_storage_v2.Models
{
    public class Documents :
        IEntity
    {
        public virtual long Id { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual long ClientId { get; set; }

        public virtual long FileFolderId { get; set; }

        public virtual long SizeFile { get; set; }

        public virtual string ShareFileWithUser { get; set; }

        public virtual DateTime DateCreate { get; set; }

        public virtual bool ShareDoc { get; set; }

        public virtual string ContentType { get; set; }
    }
}

namespace Document_storage_v2.Models.Mapping
{
    using FluentNHibernate.Mapping;

    public class DocumentsMap : ClassMap<Documents>
    {
        public DocumentsMap()
        {
            Id(u => u.Id);
            Map(d => d.FileName).Length(100);
            Map(d => d.FilePath).Length(100);
            Map(d => d.FileFolderId).Length(100);
            Map(d => d.ClientId).Length(100);
            Map(d => d.SizeFile).Length(100);
            Map(d => d.DateCreate);
            Map(d => d.ShareDoc);
            Map(d => d.ContentType).Length(100);
            //Id(u => u.ClientId);

        }
    }
}