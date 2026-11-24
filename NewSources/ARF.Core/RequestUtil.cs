using ARF.Core.Managers;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ARF.Core
{
    public class RequestUtil
    {

        protected SmtpSettings _smtpSettings;

        public T GetInstance<T>() where T : new()
        {
            if (Utils.HasOwinContext)
            {
                var instance = HttpContext.Current.GetOwinContext().Get<T>(typeof(T).FullName);
                if (instance == null)
                {
                    instance = new T();
                    HttpContext.Current.GetOwinContext().Set<T>(typeof(T).FullName, instance);
                }

                return instance;
            }

            return new T();
        }

        public TemplatesManager TemplateUtils
        {
            get
            {
                return GetInstance<TemplatesManager>();
            }
        }

        public SmtpSettings SmtpSettings
        {
            get
            {
                if (_smtpSettings == null)
                {
                    _smtpSettings = Utils.GetSmtpSettings(Utils.db);
                }

                return _smtpSettings;
            }
        }

        public int GetCoResearcherResponseTimeDays()
        {
            var coResearcherResponseSetting = Utils.db.Settings.Where(x => x.Key == ARF.Core.Constants.CORESEARCHER_RESPONSE_DAYS).FirstOrDefault();
            int coResearcherResponseTimeDays = 5; // Default Value
            if (coResearcherResponseSetting != null)
            {
                coResearcherResponseTimeDays = Convert.ToInt32(coResearcherResponseSetting.Value);
            }

            return coResearcherResponseTimeDays;
        }

        public void UpdateCoResearcherResponseFoReviewRequest()
        {
            Utils.db.Configuration.LazyLoadingEnabled = false;

            int CoResearcherResponseTimeDays = this.GetCoResearcherResponseTimeDays();
            int totalMins = CoResearcherResponseTimeDays * 24 * 60;
            var notResponded = Utils.db.RequestCoresearcherResponses.Where(x => x.SentOn != null && x.IsDeleted == false && x.Status == (byte)RequestCoresearcherResponseStatus.Pending).ToList();
            List<long> requestIds = new List<long>();
            foreach (var coresearcher in notResponded)
            {
                if (TM.Now.Subtract(coresearcher.SentOn.Value).TotalMinutes > totalMins)
                {
                    coresearcher.Status = (byte)Data.RequestCoresearcherResponseStatus.RejectedBySystem;
                    coresearcher.Updated = TM.Now;
                    coresearcher.SubmittedOn = TM.Now;
                    Utils.db.SaveChanges();
                    if (!requestIds.Contains(coresearcher.RequestId))
                    {
                        requestIds.Add(coresearcher.RequestId);
                    }
                }
            }

            if (requestIds.Count > 0)
            {
                foreach (var requestId in requestIds)
                {
                    UpdateRequestStatusAfterCoResearcherResponseForRequestUpdate(requestId, Utils.SystemAdminUserId, ArfUserRole.SystemAdmin);
                }
            }
        }

        public void UpdateRequestStatusAfterCoResearcherResponseForRequestUpdate(long requestId, string userId, ArfUserRole _role)
        {
            var coResearchersResponse = Utils.db.RequestCoresearcherResponses.Where(x => x.RequestId == requestId && x.IsDeleted == false).ToList();
            if (coResearchersResponse != null)
            {
                var returnRequestToDirector = true;
                var allResponded = true;
                foreach (var response in coResearchersResponse)
                {
                    if (response.SubmittedOn == null)
                    {
                        allResponded = false;
                    }
                }

                if (allResponded)
                {
                    var coResearchers = Utils.db.RequestCoresearchers.Where(x => x.RequestId == requestId && x.IsDeleted == false && x.ResearcherType != (byte)ResearcherRole.MainResearcher && x.ResearcherType != (byte)ResearcherRole.AssistantResearcher).ToList();
                    if (coResearchers != null && coResearchers.Count > 0)
                    {
                        foreach (var coResearcher in coResearchers)
                        {
                            if (coResearcher.DidResearcherAgree != true)
                            {
                                returnRequestToDirector = false;
                            }
                        }
                    }
                }

                if (returnRequestToDirector && allResponded)
                {
                    var request = Utils.db.Requests.Find(requestId);
                    long currentStatus = request.Status;
                    if (request.Status == (long)RequestStatus.FundingRequestPendingCoresearchersAfterRequestUpdateAfterRefereeDecision)
                    {
                        request.Status = (long)RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher;
                        request.Updated = TM.Now;
                        Utils.db.SaveChanges();

                        Utils.db.RequestHistoryLogs.Add(new RequestHistoryLog
                        {
                            RequestId = request.Id,
                            Url = "",
                            Status = currentStatus,
                            Action = (byte)(RequestAction.Approve),
                            UserAgent = "",
                            IpAddress = "",
                            Role = (byte)_role,
                            IsImpersonating = false,
                            CollegeId = request.CollegeId,
                            Created = TM.Now,
                            CreatedBy = userId,
                        });

                        Utils.db.SaveChanges();
                    }
                    else if (request.Status == (long)RequestStatus.EditFundingRequestPendingCoResearcherApproval)
                    {
                        request.Status = (long)RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch;
                        request.Updated = TM.Now;
                        Utils.db.SaveChanges();

                        Utils.db.RequestHistoryLogs.Add(new RequestHistoryLog
                        {
                            RequestId = request.Id,
                            Url = "",
                            Status = currentStatus,
                            Action = (byte)(RequestAction.Approve),
                            UserAgent = "",
                            IpAddress = "",
                            Role = (byte)_role,
                            IsImpersonating = false,
                            CollegeId = request.CollegeId,
                            Created = TM.Now,
                            CreatedBy = userId,
                        });

                        Utils.db.SaveChanges();
                    }
                }
                else if (allResponded)
                {
                    var request = Utils.db.Requests.Find(requestId);
                    long currentStatus = request.Status;
                    if (request.Status == (long)RequestStatus.FundingRequestPendingCoresearchersAfterRequestUpdateAfterRefereeDecision)
                    {
                        request.Status = (long)RequestStatus.FundingRequestPendingModificationsByResearcher;
                        request.Updated = TM.Now;
                        Utils.db.SaveChanges();

                        Utils.db.RequestHistoryLogs.Add(new RequestHistoryLog
                        {
                            RequestId = request.Id,
                            Url = "",
                            Status = currentStatus,
                            Action = (byte)(RequestAction.Approve),
                            UserAgent = "",
                            IpAddress = "",
                            Role = (byte)_role,
                            IsImpersonating = false,
                            CollegeId = request.CollegeId,
                            Created = TM.Now,
                            CreatedBy = userId,
                        });

                        Utils.db.SaveChanges();
                    }
                    else if (request.Status == (long)RequestStatus.EditFundingRequestPendingCoResearcherApproval)
                    {
                        request.Status = (long)RequestStatus.EditFundingRequestPendingSubmissionByResearcher;
                        request.Updated = TM.Now;
                        Utils.db.SaveChanges();

                        Utils.db.RequestHistoryLogs.Add(new RequestHistoryLog
                        {
                            RequestId = request.Id,
                            Url = "",
                            Status = currentStatus,
                            Action = (byte)(RequestAction.Approve),
                            UserAgent = "",
                            IpAddress = "",
                            Role = (byte)_role,
                            IsImpersonating = false,
                            CollegeId = request.CollegeId,
                            Created = TM.Now,
                            CreatedBy = userId,
                        });

                        Utils.db.SaveChanges();
                    }
                }
            }
        }
        public void DeleteRequestCoresearcherResponseRow(long requestId, long RequestCoresearcherId)
        {
            var _data = Utils.db.RequestCoresearcherResponses.Where(x => x.RequestId == requestId && x.RequestCoresearcherId == RequestCoresearcherId).ToList();
            if (_data != null && _data.Count > 0)
            {
                foreach (var _d in _data)
                {
                    _d.IsDeleted = true;
                    _d.Updated = TM.Now;
                    Utils.db.SaveChanges();
                }
            }
        }

        //public void SyncCoResearcherForRequestUpdateAfterRefereeDecision(long requestId, string userId, bool preStatus = false)
        //{
        //    var coResearchers = Utils.db.RequestCoresearchers.Where(x => x.RequestId == requestId && x.IsDeleted == false && x.ResearcherType != (byte)ResearcherRole.MainResearcher && x.ResearcherType != (byte)ResearcherRole.AssistantResearcher).ToList();
        //    if (coResearchers.Count > 0)
        //    {
        //        foreach (var coResearcher in coResearchers)
        //        {
        //            var coResearcherResponse = Utils.db.RequestCoresearcherResponses.FirstOrDefault(x => x.RequestId == requestId && x.RequestCoresearcherId == coResearcher.Id && x.IsDeleted == false);
        //            if (coResearcherResponse == null)
        //            {
        //                var _response = new RequestCoresearcherResponse
        //                {
        //                    Created = TM.Now,
        //                    Updated = TM.Now,
        //                    CreatedBy = userId,
        //                    UpdatedBy = userId,
        //                    RequestId = requestId,
        //                    RequestCoresearcherId = coResearcher.Id,
        //                    IsDeleted = false,
        //                    AgreementType = (byte)RequestCoresearcherResponseAgreementType.RequestUpdateAfterRefereeDecision,
        //                    Status = (byte)RequestCoresearcherResponseStatus.Pending
        //                };
        //                Utils.db.RequestCoresearcherResponses.Add(_response);
        //                Utils.db.SaveChanges();
        //            }
        //            else
        //            {
        //                coResearcherResponse.IsDeleted = coResearcher.IsDeleted;
        //                coResearcherResponse.UpdatedBy = userId;
        //                coResearcherResponse.Updated = TM.Now;
        //                if (preStatus)
        //                {
        //                    coResearcherResponse.Status = (byte)RequestCoresearcherResponseStatus.Pending;
        //                    coResearcherResponse.SentOn = null;
        //                    coResearcherResponse.ReceivedOn = null;
        //                    coResearcherResponse.SubmittedOn = null;
        //                }
        //                Utils.db.SaveChanges();
        //            }
        //        }
        //    }
        //}

        public bool isAnyCoResearcherRejected(long requestId, byte AgreementType)
        {
            var _count = Utils.db.RequestCoresearcherResponses.Where(x => x.RequestId == requestId && x.AgreementType == AgreementType && x.IsDeleted == false && (x.Status == (byte)RequestCoresearcherResponseStatus.Rejected || x.Status == (byte)RequestCoresearcherResponseStatus.RejectedBySystem)).Count();

            return _count > 0;
        }

        public string GetResearcherContractStartDate(Request request)
        {
            var contract_startdate = Utils.db.Settings.FirstOrDefault(x => x.Key == Constants.FUNDING_REQUEST_CONTRACT_START_DATE);
            string contractStartDate = request.ResearcherContractStartDate == null ? (contract_startdate != null ? contract_startdate.Value : "") : request.ResearcherContractStartDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-Us"));
            return contractStartDate;
        }

        public string GetResearcherContractText(Request request)
        {
            string contract = request.ResearcherContractText;
            if (String.IsNullOrEmpty(contract))
            {
                var mainResearcher = Utils.db.Researchers.Where(r => r.UserId == request.CreatedBy).FirstOrDefault();
                if (mainResearcher != null)
                {
                    var contract_arabic = Utils.db.Settings.FirstOrDefault(x => x.Key == Constants.FUNDING_REQUEST_CONTRACT_TEMPLATE_ARABIC);
                    if (contract_arabic != null)
                    {                        
                        string researchTitle = request.Title;
                        contract = contract_arabic.Value;
                        string projectCost = request.TotalCostForProject.ToString();
                        if (request.MaterialsTotalCost != null && request.MaterialsTotalCost != 0)
                        {
                            projectCost = request.MaterialsTotalCost.ToString() + " + " + (request.FirstPeriodTotalCost.Value + request.SecondPeriodTotalCost.Value) + " = " + request.TotalCostForProject.ToString();
                        }
                        var country = Utils.db.Countries.ToList().Where(x => x.Id == mainResearcher.NationalityId).FirstOrDefault();
                        var countryName = String.Empty;
                        if (country != null)
                        {
                            countryName = country.NameArabic;
                        }
                        string SecretaryMeetingDate = request.SecretaryMeetingDate != null ? request.SecretaryMeetingDate.Value.FormatDate() : "";
                        string ContractApprovalDate = request.ContractApprovalDate != null ? request.ContractApprovalDate.Value.FormatDate() : "";
                        var coResearchersNames = String.Empty;
                        if (request.RequestCoresearchers != null)
                        {
                            var index = 1;
                            var RequestCoresearchers = request.RequestCoresearchers.ToList();
                            foreach(var coresearcher in RequestCoresearchers)
                            {
                                if (coresearcher.ResearcherType != (byte)ResearcherRole.MainResearcher)
                                {
                                    coResearchersNames = coResearchersNames + index.ToString() + ". " + coresearcher.Researcher.GetFullName() + " - " + coresearcher.Researcher.NationalId + "<br/>";
                                    index++;
                                }
                            }
                        }

                        contract = contract.Replace("{FullNameMainResearcher}", mainResearcher.GetFullName())
                                           .Replace("{NationalIdMainResearcher}", mainResearcher.NationalId)
                                           .Replace("{PhoneNumberMainResearcher}", mainResearcher.MobileNo)
                                           .Replace("{EmailMainResearcher}", mainResearcher.Email)
                                           .Replace("{NationalityMainResearcher}", countryName)
                                           .Replace("{ProjectTitle}", researchTitle)
                                           .Replace("{ProjectBudget}", request.TotalCostForProject.ToString())
                                           .Replace("{ProjectDuration}", request.Duration.ToString())
                                           .Replace("{SecretaryMeetingDate}", SecretaryMeetingDate)
                                           //.Replace("{ContractApprovalDate}", ContractApprovalDate)
                                           .Replace("{ProjectSequenceNumber}", request.UniqueId)
                                           .Replace("{ProjectDuration}", request.Duration.Value.ToString())
                                           .Replace("{ProjectBudget}", projectCost)
                                           .Replace("{CollegeName}", request.College.NameArabic)
                                           .Replace("{DepartmentName}", request.Department.NameArabic)                                           
                                           .Replace("{ProjectStartDate}", this.GetResearcherContractStartDate(request))
                                           .Replace("{CoResearchersNames}", coResearchersNames)
                                           ;
                    }
                }
            }

            return contract;
        }

        public void sendNotificationToCoResearcherForRequestUpdateAfterRefereeDecision(long requestId, string userId)
        {
            var request = Utils.db.Requests.Where(x => x.Id == requestId && x.IsDeleted == false).FirstOrDefault();
            if (request != null)
            {
                string baseURL = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];

                var coResearchers = new List<RequestCoresearcher>();

                coResearchers = Utils.db.RequestCoresearchers.Include("Researcher").Where(x => x.RequestId == requestId && x.IsDeleted == false && x.ResearcherType != (byte)ResearcherRole.MainResearcher && x.ResearcherType != (byte)ResearcherRole.AssistantResearcher).ToList();
                if (coResearchers.Count > 0)
                {
                    Template _smsTemplte = TemplateUtils[TemplateKey.CoResearcherUpdateRequestSMSNotification]; ;
                    Template _emailTemplte = TemplateUtils[TemplateKey.CoResearcherUpdateRequestEmailNotification]; ;

                    foreach (var coResearcher in coResearchers)
                    {
                        var _response = new RequestCoresearcherResponse
                        {
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            RequestId = requestId,
                            RequestCoresearcherId = coResearcher.Id,
                            IsDeleted = false,
                            AgreementType = (byte)RequestCoresearcherResponseAgreementType.RequestUpdateAfterRefereeDecision,
                            Status = (byte)RequestCoresearcherResponseStatus.Pending,
                            SentOn = TM.Now
                        };
                        Utils.db.RequestCoresearcherResponses.Add(_response);
                        Utils.db.SaveChanges();

                        coResearcher.DidResearcherAgree = false;
                        coResearcher.Updated = TM.Now;
                        coResearcher.UpdatedBy = userId;
                        Utils.db.SaveChanges();
                        var fullName = coResearcher.Researcher.GetFullName();
                        String mainResearcherFullName = String.Empty;
                        var mainResearcher = Utils.db.Users.Find(request.CreatedBy);
                        if (mainResearcher != null)
                        {
                            mainResearcherFullName = mainResearcher.FullName;
                        }

                        string _publicLinkHash = Utils.Encrypt(requestId.ToString() + Core.Constants.ExternalRefereeParamsConnector.ToString() + _response.Id + Core.Constants.ExternalRefereeParamsConnector.ToString() + coResearcher.Researcher.Email);
                        string _publicLink = baseURL + "/Account/RequestCoResearcherPublicLink?_h=" + HttpUtility.UrlEncode(_publicLinkHash);
                        if (!String.IsNullOrEmpty(coResearcher.Researcher.MobileNo))
                        {
                            string body = Utils.IsUserLanguageArabic ? _smsTemplte.BodyArabic : _smsTemplte.BodyEnglish;
                            body = body.Replace("{FullName}", fullName);
                            body = body.Replace("{RequestTitle}", request.Title);
                            SmsService.SendMessage(body, new string[] { coResearcher.Researcher.MobileNo });
                        }

                        if (_emailTemplte != null)
                        {
                            string EmailSubject = _emailTemplte.SubjectArabic;
                            string EmailBody = _emailTemplte.BodyArabic;                            
                            
                            EmailBody = EmailBody.Replace(" {FullName}", fullName).
                                        Replace("{RequestTitle}", request.Title).
                                        Replace("{MainResearcherFullName}", mainResearcherFullName).
                                        Replace("{REQUEST_DETAIL_PUBLIC_LINK}", _publicLink);

                            EmailService.SendMessage(SmtpSettings, EmailSubject, EmailBody, new MailAddress(coResearcher.Researcher.Email), null, null);
                        }

                    }
                }
            }
        }

        public void sendNotificationToCoResearcherAfterEditingRequest(long requestId, string userId)
        {
            var request = Utils.db.Requests.Where(x => x.Id == requestId && x.IsDeleted == false).FirstOrDefault();
            if (request != null)
            {
                string baseURL = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];

                var coResearchers = new List<RequestCoresearcher>();

                coResearchers = Utils.db.RequestCoresearchers.Include("Researcher").Where(x => x.RequestId == requestId && x.IsDeleted == false && x.ResearcherType != (byte)ResearcherRole.MainResearcher && x.ResearcherType != (byte)ResearcherRole.AssistantResearcher).ToList();
                if (coResearchers.Count > 0)
                {
                    Template _smsTemplte = TemplateUtils[TemplateKey.CoResearcherUpdateRequestSMSNotification]; ;
                    Template _emailTemplte = TemplateUtils[TemplateKey.CoResearcherUpdateRequestEmailNotification]; ;

                    foreach (var coResearcher in coResearchers)
                    {
                        var _response = new RequestCoresearcherResponse
                        {
                            Created = TM.Now,
                            Updated = TM.Now,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            RequestId = requestId,
                            RequestCoresearcherId = coResearcher.Id,
                            IsDeleted = false,
                            AgreementType = (byte)RequestCoresearcherResponseAgreementType.RequestUpdateAfterRefereeDecision,
                            Status = (byte)RequestCoresearcherResponseStatus.Pending,
                            SentOn = TM.Now
                        };
                        Utils.db.RequestCoresearcherResponses.Add(_response);
                        Utils.db.SaveChanges();

                        coResearcher.DidResearcherAgree = false;
                        coResearcher.Updated = TM.Now;
                        coResearcher.UpdatedBy = userId;
                        Utils.db.SaveChanges();
                        var fullName = coResearcher.Researcher.GetFullName();
                        String mainResearcherFullName = String.Empty;
                        var mainResearcher = Utils.db.Users.Find(request.CreatedBy);
                        if (mainResearcher != null)
                        {
                            mainResearcherFullName = mainResearcher.FullName;
                        }

                        string _publicLinkHash = Utils.Encrypt(requestId.ToString() + Core.Constants.ExternalRefereeParamsConnector.ToString() + _response.Id + Core.Constants.ExternalRefereeParamsConnector.ToString() + coResearcher.Researcher.Email);
                        string _publicLink = baseURL + "/Account/RequestCoResearcherPublicLink?_h=" + HttpUtility.UrlEncode(_publicLinkHash);
                        if (!String.IsNullOrEmpty(coResearcher.Researcher.MobileNo))
                        {
                            string body = Utils.IsUserLanguageArabic ? _smsTemplte.BodyArabic : _smsTemplte.BodyEnglish;
                            body = body.Replace("{FullName}", fullName);
                            body = body.Replace("{RequestTitle}", request.Title);
                            SmsService.SendMessage(body, new string[] { coResearcher.Researcher.MobileNo });
                        }

                        if (_emailTemplte != null)
                        {
                            string EmailSubject = _emailTemplte.SubjectArabic;
                            string EmailBody = _emailTemplte.BodyArabic;

                            EmailBody = EmailBody.Replace(" {FullName}", fullName).
                                        Replace("{RequestTitle}", request.Title).
                                        Replace("{MainResearcherFullName}", mainResearcherFullName).
                                        Replace("{REQUEST_DETAIL_PUBLIC_LINK}", _publicLink);

                            EmailService.SendMessage(SmtpSettings, EmailSubject, EmailBody, new MailAddress(coResearcher.Researcher.Email), null, null);
                        }

                    }
                }
            }
        }


        public string GetFundingRequestContractHeader(Request request, string FileName)
        {
            string header = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>" + FileName + "</title></head><body>";

            return header;
        }

        public string GetFundingRequestContractFooter(Request request, string fileName)
        {
            string footer = "</body></html>";

            return footer;
        }
    }
}
