using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Document_storage_v2.Models;

using NHibernate;
using NHibernate.Criterion;
using Document_storage_v2.Auth;
using NHibernate.SqlCommand;

namespace Document_storage_v2._0.Controllers
{
    public class DocumentsController :
        BaseController
    {

        readonly IRepository<Documents> docRepo;
        readonly IRepository<Users> usersRepo;
        readonly IRepository<Folder> folderRepo;

        public DocumentsController(ISession session) :
    base(session)
        {
            docRepo = CreateRepo<Documents>();
            folderRepo = CreateRepo<Folder>();
            usersRepo = CreateRepo<Users>();
        }

        public ActionResult ShowShareDocuments(Folder model)
        {
            return View(title: "Документы",
                    model: new ShareDocuments
                    {
                        Documents = docRepo
                               .CreateCriteria()
                               .Add(Restrictions.Eq("ClientId", CurrentUser.Id))
                               .Add(Restrictions.Eq("ShareDoc", true))
                               .List<Documents>()
                    });
        }

        public ActionResult NoApplyFile(long Id = 0)
        {
            Documents document = docRepo.CreateCriteria()
                 .Add(Restrictions.Eq("Id", Id))
                 .Add(Restrictions.Eq("ShareDoc", true))
                 .Add(Restrictions.Eq("ClientId", CurrentUser.Id))
                 .UniqueResult<Documents>();
  
            docRepo.Transaction(r => r.Delete(document));

            return RedirectToAction("ShowShareDocuments");
        }

        public ActionResult ApplyFile(long Id = 0 )
        {
            if (Request.IsAuthenticated && Id != 0)
            {
                Documents document = docRepo.CreateCriteria()
                     .Add(Restrictions.Eq("Id", Id))
                     .Add(Restrictions.Eq("ShareDoc", true))
                     .Add(Restrictions.Eq("ClientId", CurrentUser.Id))
                     .UniqueResult<Documents>();

                string FullPathOld = Request.MapPath(document.FilePath);

                if (document.ClientId == CurrentUser.Id && System.IO.File.Exists(FullPathOld))
                {
                    string fileName = Guid.NewGuid().ToString();

                    string fileExtension = System.IO.Path.GetExtension(document.FileName);

                    Documents newDocument = new Documents()
                    {
                        FilePath = "~/Files/" + fileName + fileExtension,
                        ClientId = CurrentUser.Id,
                        FileName = document.FileName,
                        FileFolderId = 0,
                        ShareDoc = false,
                        SizeFile = document.SizeFile,
                        DateCreate = DateTime.Now,
                        ContentType = document.ContentType
                    };

                    string FullPathNew = Request.MapPath(newDocument.FilePath);

                    System.IO.File.Copy(FullPathOld, FullPathNew);

                    docRepo.Transaction(r => r.Save(newDocument));

                    docRepo.Transaction(r => r.Delete(document));

                }
            }



            return RedirectToAction("ShowShareDocuments");
        }

        // GET: Documents
        public ActionResult Documents(long Id = 0, string SortOrder = "FileName")
        {
            if (Request.IsAuthenticated)
            {
                var user = CurrentUser;

                return View(title: "Документы",
                    model: new DocumentsViewModels
                    {
                        Owner = user,
                        ParentId = Id,
                        Documents = docRepo
                                 .CreateCriteria()
                                 .Add(Restrictions.Eq("ClientId", user.Id))
                                 .Add(Restrictions.Eq("FileFolderId", Id))
                                 .Add(Restrictions.Eq("ShareDoc", false))
                                 .AddOrder(Order.Asc(SortOrder))
                                 .List<Documents>() ,

                        Folder = folderRepo.Get(
                            criterions: new ICriterion[] {
                                    Restrictions.Eq("ClientId", user.Id),
                                    Restrictions.Eq("FolderParent", Id)
                        })
                    });
            }
            return View();
        }

        public ActionResult DeleteDocument(long IdDoc = 0, long folderId = 0)
        {
            if (Request.IsAuthenticated && IdDoc != 0)
            {
                Documents document = docRepo.CreateCriteria()
                 .Add(Restrictions.Eq("Id", IdDoc))
                 .UniqueResult<Documents>();

                docRepo.Transaction(r => r.Delete(document));
            }

            return RedirectToAction("Documents", new { @Id = folderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFolder(Folder model)
        {
            if (ModelState.IsValid && Request.IsAuthenticated)
            {
                model.ClientId = CurrentUser.Id;

                folderRepo.Transaction(r => r.Save(model));

            }

            return RedirectToAction("Documents", new { @Id = model.FolderParent });
        }

        public ActionResult AddDocument()
        {
            return View();
        }

        private void SplitAndShareDoc(Documents model, HttpPostedFileBase File)
        {
            List<string> UsersShare = model.ShareFileWithUser.Split(new char[] { ',' }).OfType<string>().ToList(); ;

            if (UsersShare.Count > 0)
            {
                model.FilePath = "~/Files/cache/" + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(File.FileName);

                model.ShareDoc = true;

                File.SaveAs(Server.MapPath(model.FilePath));



                foreach (string user in UsersShare)
                {
                    Users userShare = userRepo.CreateCriteria()
                         .Add(Restrictions.Eq("UserLogin", user.Trim()))
                         .UniqueResult<Users>();
                   
                    

                    if (userShare.Id != 0)
                    {
                        model.ClientId = userShare.Id;
                        docRepo.Transaction(r => r.Save(model));
                    }

                    
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocument(Documents model, HttpPostedFileBase File)
        {
            if (ModelState.IsValid && Request.IsAuthenticated)
            {
                string fileName = Guid.NewGuid().ToString();

                string fileExtension = System.IO.Path.GetExtension(File.FileName);

                if (model.FileName != null)
                {
                    model.FileName = model.FileName + fileExtension;
                }
                else
                {
                    model.FileName = File.FileName;
                }

                model.SizeFile = File.ContentLength;

                model.ClientId = CurrentUser.Id;

                model.DateCreate = DateTime.Now;

                model.ContentType = File.ContentType;

                if (!String.IsNullOrWhiteSpace(model.ShareFileWithUser))
                {
                    Documents LocalModel = new Documents()
                    {
                        ClientId = model.ClientId,
                        DateCreate = model.DateCreate,
                        ContentType = model.ContentType,
                        SizeFile = model.SizeFile,
                        FileName = model.FileName,
                        ShareFileWithUser = model.ShareFileWithUser
                    };
                    SplitAndShareDoc(LocalModel, File);
                }

                model.FilePath = "~/Files/" + fileName + fileExtension;

                model.ShareDoc = false;

                File.SaveAs(Server.MapPath(model.FilePath));

                docRepo.Transaction(r => r.Save(model));

            }

            return RedirectToAction("Documents", new { @Id = model.FileFolderId });
        }

        public ActionResult LoadFile(long Id = 0)
        {
            if (Request.IsAuthenticated && Id != 0)
            {

                Documents document = docRepo.CreateCriteria()
                 .Add(Restrictions.Eq("Id", Id))
                 .UniqueResult<Documents>();

                return File(document.FilePath, document.ContentType, document.FileName);

            }
            return View("Error");
        }
    }
}