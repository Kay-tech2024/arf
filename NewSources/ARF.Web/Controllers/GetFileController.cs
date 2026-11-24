using ARF.Web.App_Start.Code;
using MimeTypes;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    [AllowAnonymous]
    public class GetFileController : BaseController
    {
        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.Write(Resources.ArfStrings.this_file_is_incorrect);
            }

            var file = DbContext.UserSessionFiles.Where
                    (c => c.SessionId == UserSession.Id)
                   .Where(a => a.Id.ToString() == id).Select(c => new FileToBeDownloaded
                   {
                       FilePath = c.FilePath,
                       FileName = c.FileName
                   }).FirstOrDefault();

            // logged in users
            if (file == null && CurrentUser != null)
            {
                file = DbContext.RequestFiles.Where(c => !IsCollegeUser || (CurrentUser.CollegeId != null &&
                        (
                            c.Request.CollegeId == CurrentUser.CollegeId
                        )
                    )).Where(a => a.Id.ToString() == id).Select(c => new FileToBeDownloaded
                    {
                        FilePath = c.FilePath,
                        FileName = c.FileName
                    }).FirstOrDefault();


                if (file == null)
                {
                    file = DbContext.UserFiles.Where(c => !IsCollegeUser || (CurrentUser.CollegeId != null && c.User.CollegeId == CurrentUser.CollegeId)).Where(a => a.Id.ToString() == id).Select(c => new FileToBeDownloaded
                    {
                        FilePath = c.FilePath,
                        FileName = c.FileName
                    }).FirstOrDefault();
                }

            }

            if (file == null)
            {
                Response.Write(Resources.ArfStrings.this_file_is_incorrect);
                Response.End();
                return null;
            }

            if (!System.IO.File.Exists(file.FilePath))
            {
                Response.Write(Resources.ArfStrings.this_file_is_not_exist);
                Response.End();
                return null;
            }

            return File(file.FilePath, MimeTypeMap.GetMimeType(Path.GetExtension(file.FilePath)), file.FileName);
        }
    }

    public class FileToBeDownloaded
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool Isdownloaded { get; set; }
    }
}