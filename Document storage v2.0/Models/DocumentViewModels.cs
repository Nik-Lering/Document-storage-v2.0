using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Document_storage_v2.Models
{
    public class ShareDocuments
    {
        [Required]
        public ICollection<Documents> Documents { get; set; }

    }

    public class DocumentsViewModels
    {
        [Required]
        public Users Owner { get; set; }

        [Required]
        public long ParentId { get; set; }

        [Required]
        public ICollection<Documents> Documents { get; set; }

        [Required]
        public ICollection<Folder> Folder { get; set; }
    }

    public class AddDocument
    {
        [Required]
        [Display(Name = "Путь к файлу")]
        public string File { get; set; }

        [Display(Name = "Название файла")]
        public string FileName { get; set; }

        [Display(Name = "Поделиться с: (Перечислите через запятую)")]
        public string ShareFileWithUser { get; set; }

    }

    public class AddFolder
    {
        [Display(Name = "Название папки")]
        public long ParentId { get; set; }

        [Required]
        [Display(Name = "Название папки")]
        public string FolderName { get; set; }

    }
}