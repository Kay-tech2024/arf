using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Controllers
{
    public class LettersController : BaseController
    {
        // GET: Letters
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Detail(Guid id)
        {

            return View(DbContext.Letters.Find(id));
        }


        public ActionResult DetailForD(Guid id)
        {
            Letter letter = DbContext.Letters.Find(id);
            letter.IsSeen = true;
            DbContext.SaveChanges();
            return View(DbContext.Letters.Find(id));
        }


        public ActionResult DetailForR(Guid id)
        {
            Letter letter = DbContext.Letters.Find(id);
            letter.IsSeen = true;
            DbContext.SaveChanges();
            return View(DbContext.Letters.Find(id));
        }

        public ActionResult DetailForDz(Guid id)
        {
            Letter letter = DbContext.Letters.Find(id);
            letter.IsSeen = true;
            DbContext.SaveChanges();
            return View(DbContext.Letters.Find(id));
        }

        public ActionResult GetDesignLetterForDz()
        {
            return View(DbContext.Letters.Where(l => l.Type == (int)LetterTypes.RequestDesign).OrderByDescending(t => t.RefId).ToList()); // for   مدير البرنامج
        }
        public ActionResult GetFundingLetterForR()
        {
            return View(DbContext.Letters.Where(l => l.Type == (int)LetterTypes.FundingLetterToDeanshipResearch).OrderByDescending(t => t.RefId).ToList()); // for  وكيل العمادات
        }

        public ActionResult GetFundingLetterRR()
        {
            return View(DbContext.Letters.Where(l => l.Type == (int)LetterTypes.FundingLetterToDeanshipResearch).OrderByDescending(t => t.RefId).ToList()); // for وكيل العمادات
        }
        public ActionResult CreateFundingLetterForR()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFundingLetterForR(Letter letter)
        {
            try
            {
                DbContext.Letters.Add(new Letter()
                {

                    Id = Guid.NewGuid(),
                    RoleId = "1B62AB2A-A1AA-499F-935D-FF2352FCCB86", // dean of deanship
                    Text = letter.Text,
                    Type = (byte)LetterTypes.FundingLetterToDeanshipResearch,
                    IsSeen = false,
                });
                DbContext.SaveChanges();

                TempData["message"] = LangHelpers.Instance["AddedSuccessfully"];
                TempData["message_type"] = "success";

                return RedirectToAction("GetFundingLetterForR");
            }
            catch (Exception ex)
            {

                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return View();
            }

        }

        public ActionResult GetFundingLetter()
        {
            return View(DbContext.Letters.Where(l => l.Type == (int)LetterTypes.FundingLetter).OrderByDescending(l => l.RefId).ToList()); // for مدير البرنامج
        }

        public ActionResult FundingLetter()
        {
            return View(DbContext.Letters.Where(l => l.RoleId == "14B8AC62-3917-49DB-896B-0C0C54C026B7").OrderByDescending(l => l.RefId).ToList()); // deanship 101
        }

        public ActionResult CreateFundingLetter()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFundingLetter(Letter letter, string date, string dateEnd)
        {
            try
            {
                DbContext.Letters.Add(new Letter()
                {

                    Id = Guid.NewGuid(),
                    RoleId = "14B8AC62-3917-49DB-896B-0C0C54C026B7", // dean of deanship
                    Text =     Resources.ArfStrings.funding_period_starting_date + date.ToString() + "  ------- " + letter.Text + "  ------------ " + Resources.ArfStrings.funding_period_finishing_date+ dateEnd.ToString(),
                    Type = (byte)LetterTypes.FundingLetter,
                    IsSeen = false,
                });
                DbContext.SaveChanges();

                TempData["message"] = LangHelpers.Instance["AddedSuccessfully"];
                TempData["message_type"] = "success";

                return RedirectToAction("FundingLetter");
            }
            catch (Exception ex)
            {

                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return View();
            }

        }
        public ActionResult GetDesignLetter()
        {
            return View(DbContext.Letters.Where(l => l.Type == (int)LetterTypes.RequestDesign).OrderByDescending(t => t.RefId).ToList()); // for   مدير البرنامج
        }
        public ActionResult CreateRequestDesign()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRequestDesign(Letter letter)
        {
            try
            {
                DbContext.Letters.Add(new Letter()
                {

                    Id = Guid.NewGuid(),
                    RoleId = "72DEE233-E95C-4B2F-850C-C6BBF72DDAEE", // dean of deanship
                    Text = letter.Text,
                    Type = (byte)LetterTypes.RequestDesign,
                    IsSeen = false,
                });
                DbContext.SaveChanges();

                TempData["message"] = LangHelpers.Instance["AddedSuccessfully"];
                TempData["message_type"] = "success";

                return RedirectToAction("GetDesignLetter");
            }
            catch (Exception ex)
            {

                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return View();
            }
        }

        public ActionResult GetAllProposedDesignForD()
        {
            return View(DbContext.Designs.Where(d => d.Status == 0).ToList()); // for dean
        }


        public ActionResult GetAllProposedDesign()
        {
            return View(DbContext.Designs.Where(d => d.Status == 0).ToList()); // for desinger
        }


        public ActionResult GetAllAcceptedDesign()
        {
            return View(DbContext.Designs.Where(d => d.Status == (int)SubmitTheProposedDesignStatus.Approve).ToList());
        }

        public ActionResult SubmitTheProposedDesign()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitTheProposedDesign(Design design, HttpPostedFileBase file)
        {
            try
            {
                Guid guid = Guid.NewGuid();
                // upload files
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    string fileName = guid.ToString();

                    var path = Path.Combine(Server.MapPath("~/UploadedFile/ProposedImage/"), fileName + fileExtension);


                    file.SaveAs(path);
                    design.Image = fileName + fileExtension;
                }

                Design Newdesign = new Design();

                Newdesign.Id = guid;
                Newdesign.Image = design.Image;
                Newdesign.Status = 0;
                Newdesign.Text = design.Text;

                DbContext.Designs.Add(Newdesign);
                DbContext.SaveChanges();

                TempData["message"] = LangHelpers.Instance["AddedSuccessfully"];
                TempData["message_type"] = "success";

                return RedirectToAction("GetAllProposedDesign");
            }
            catch (Exception ex)
            {

                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return View();
            }
        }



        // reject and accept designf
        public ActionResult AcceptDesign(Guid id)
        {
            try
            {
                Design design = DbContext.Designs.Find(id);
                design.Status = (int)SubmitTheProposedDesignStatus.Approve;

                DbContext.Entry<Design>(design).State = System.Data.Entity.EntityState.Modified;
                DbContext.SaveChanges();

                return RedirectToAction("GetAllProposedDesignForD");

            }
            catch (Exception e)
            {
                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return RedirectToAction("GetAllProposedDesignForD");
            }
        }

        public ActionResult RejectDesign(Guid id)
        {
            try
            {
                Design design = DbContext.Designs.Find(id);
                design.Status = (int)SubmitTheProposedDesignStatus.Rejected;
                DbContext.Entry<Design>(design).State = System.Data.Entity.EntityState.Modified;
                DbContext.SaveChanges();
                return RedirectToAction("GetAllProposedDesignForD");
            }
            catch (Exception e)
            {
                TempData["message"] = LangHelpers.Instance["AnErrorOccurredDuringTheAddOperation"];
                TempData["message_type"] = "danger";
                return RedirectToAction("GetAllProposedDesignForD");
            }
        }



        public ActionResult Funding()
        {
            return View();
        }
    }
}