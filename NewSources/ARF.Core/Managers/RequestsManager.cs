using ARF.Core.Models;
using ARF.Core.Workflows;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace ARF.Core.Managers
{
    public class RequestsManager : AbstractManager
    {
        public Request GetSingleRequest(long? id)
        {
            return RequestUtils.GetRequests(null).SingleOrDefault(c => c.Id == id);
        }

        public IQueryable<Request> GetRequests(long? CollegeId = null, long? groupId = null, string userId = null, Data.RequestType? type = null, RequestTab? status = null, string queryStr = null, int? pageNo = null, int? pageSize = null, string sortBy = null, string sortDir = null, Translator translator = null)
        {
            long total = 0;
            return GetRequests(CollegeId, groupId, userId, type, status, queryStr, pageNo, pageSize, sortBy, sortDir, translator, null, out total);
        }

        public IQueryable<Request> GetRequests(long? collegeId, long? CollegeId, string userId, Data.RequestType? type, RequestTab? status, string queryStr, int? pageNo, int? pageSize, string sortBy, string sortDir, Translator translator, FilterModel filter, out long total)
        {
            total = 0;
            if (Roles == null || Roles.Length == 0)
                return new List<Request>().AsQueryable();

            var subpredicate = PredicateBuilder.BaseAnd<Request>();

            if (CollegeId != null)
            {
                subpredicate = subpredicate.And(r => r.CreatedByUser.CollegeId == CollegeId);
            }

            if (userId != null)
            {
                subpredicate = subpredicate.And(r => r.CreatedBy == userId);
            }

            if (type != null)
            {
                subpredicate = subpredicate.And(r => r.Type == (byte)type);
            }

            if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                subpredicate = subpredicate.And(r => r.CreatedBy == CurrentUser.Id || (r.PublicationRequest.CreatedBy == CurrentUser.Id && r.Type != (byte)Data.RequestType.RefereeReward));
            }
            else if (Utils.HasRolesAny(ArfUserRole.Designer))
            {
                subpredicate = subpredicate.And(r => r.Type == (byte)Data.RequestType.DesignRequest);
            }
            else if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
            {
                subpredicate = subpredicate.And(r => r.Status != (long)RequestStatus.NotSubmitted && r.Status != (long)RequestStatus.NotStarted &&
                    (r.Type == (byte)Data.RequestType.AdvertisementLetterForCollegeDeans ||
                    r.Type == (byte)Data.RequestType.FundingRequest ||
                    r.Type == (byte)Data.RequestType.MaterialRequest ||
                    r.Type == (byte)Data.RequestType.EditFundingRequest ||
                    r.Type == (byte)Data.RequestType.FulltermProgressReport ||
                    r.Type == (byte)Data.RequestType.MidTermProgressReport)
                );
            }
            else if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheDeanshipOfResearchCentersAndDatabases))
            {
                subpredicate = subpredicate.And(r =>
                    r.Type == (byte)Data.RequestType.FundingRequest ||
                    r.Type == (byte)Data.RequestType.FulltermProgressReport ||
                    r.Type == (byte)Data.RequestType.MidTermProgressReport ||
                    r.Type == (byte)Data.RequestType.MidTermProgressReport
                );
            }

            if (!Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                subpredicate = subpredicate.And(r => r.Status != (long)RequestStatus.Canceled && r.Status != (long)RequestStatus.NotSubmitted);

                if (Utils.HasRolesAny(ArfUserRole.Referee))
                {
                    subpredicate = subpredicate.And(r => r.RequestReferees.Count(rr => rr.Referee.UserId == CurrentUser.Id && rr.RefereeId == rr.Referee.Id && rr.RequestReviewSentOn != null) > 0);
                }
                else if (Utils.HasRolesAny(ArfUserRole.HeadOfTheStandingCommitteeForFinancingResearchProjects))
                {
                    subpredicate = subpredicate.And(r => r.RequestUsers.Count(u => u.UserId == CurrentUser.Id) > 0 ||
                        r.Status == (long)RequestStatus.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects
                    );
                }
                else if (Utils.HasRolesAny(ArfUserRole.ResearchUnitMember))
                {
                    subpredicate = subpredicate.And(r => r.Status == (long)RequestStatus.FundingRequestPendingCollegeCommittee || r.Status == (long)RequestStatus.EditFundingRequestPendingCollegeCommittee);
                }
                else if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                {
                    subpredicate = subpredicate.And(r => r.Type == (byte)RequestType.FundingRequest || r.Type == (byte)RequestType.EditFundingRequest);
                }
                else if (Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects))
                {
                    subpredicate = subpredicate.And(r => (r.Type == (byte)RequestType.FundingRequest || r.Type == (byte)RequestType.EditFundingRequest) &&
                                                    (r.RequestUsers.Count(u => u.UserId == CurrentUser.Id) > 0 ||
                                                        r.Status == (long)RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects ||
                                                        r.Status == (long)RequestStatus.EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects
                                                    )
                    );
                }
                /*else if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                {
                    subpredicate = subpredicate.And(r => r.RequestUsers.Count(u => u.UserId == CurrentUser.Id) > 0 ||
                        r.Status == (long)RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch ||
                        r.Status == (long)RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies ||
                        r.Status == (long)RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch2 ||
                        r.Status == (long)RequestStatus.MaterialRequestReceivedByVicePresidentForHigherStudiesAndScientificResearch ||
                        r.Status == (long)RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch
                    );
                }*/

            }
            if (Utils.IsCollegeUser)
            {
                subpredicate = subpredicate.And(r => r.CollegeId == CurrentUser.CollegeId);
            }

            var query = DbContext.Requests.AsQueryable();

            if (status != null)
            {
                switch (status.Value)
                {
                    case RequestTab.Pending:

                        query = query.Where(r => r.Status != (long)RequestStatus.Completed && r.Status != (long)RequestStatus.Rejected && r.Status != (long)RequestStatus.Canceled);
                        break;

                    case RequestTab.Archive:
                        query = query.Where(r => r.Status == (long)RequestStatus.Completed || r.Status == (long)RequestStatus.Rejected || r.Status == (long)RequestStatus.Canceled);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                var predicate2 = PredicateBuilder.BaseOr<Request>();

                predicate2 = predicate2.Or(r => r.Title.Contains(queryStr));
                queryStr = queryStr.Trim().ToLower();
                long t = 0;

                if (IsValidNationalId(queryStr))
                {
                    predicate2 = predicate2.Or(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.NationalId == queryStr);
                }
                else if (long.TryParse(queryStr, out t))
                {
                    predicate2 = predicate2.Or(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.EmployeeId.Contains(queryStr));
                }
                else
                {
                    predicate2 = predicate2.Or(r => r.CreatedByUser.FullName.Contains(queryStr));

                    var names = Utils.GetNameComponents(queryStr);
                    if (names != null && names.FirstName != null)
                    {
                        predicate2 = predicate2.Or(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.FirstName == names.FirstName && (names.LastName == null || r.CreatedByUser.UserProfile.LastName == names.LastName));
                    }
                }

                if (translator != null)
                {
                    {
                        var requestTypes = Enum.GetValues(typeof(Data.RequestType));

                        var matchingTypes = new List<Data.RequestType>();

                        foreach (var r in requestTypes)
                        {
                            var arabic = translator.GetLangValueByString(r.ToString(), true, "ar");
                            var english = translator.GetLangValueByString(r.ToString(), true, "en");

                            if (arabic != null && arabic.Contains(queryStr))
                                matchingTypes.Add((Data.RequestType)r);
                            else if (english != null && english.Contains(queryStr))
                                matchingTypes.Add((Data.RequestType)r);
                        }

                        if (matchingTypes.Count > 0)
                        {
                            foreach (var x in matchingTypes)
                            {
                                predicate2 = predicate2.Or(a => a.Type == (byte)x);
                            }
                        }
                    }

                    {
                        var statuses = Enum.GetValues(typeof(RequestStatus));

                        var matchingStatuses = new List<RequestStatus>();

                        foreach (var r in statuses)
                        {
                            var arabic = translator.GetLangValueByString(r.ToString(), true, "ar");
                            var english = translator.GetLangValueByString(r.ToString(), true, "en");

                            if (arabic != null && arabic.Contains(queryStr))
                                matchingStatuses.Add((RequestStatus)r);
                            else if (english != null && english.Contains(queryStr))
                                matchingStatuses.Add((RequestStatus)r);
                        }

                        if (matchingStatuses.Count > 0)
                        {
                            foreach (var x in matchingStatuses)
                            {
                                predicate2 = predicate2.Or(a => a.Status == (long)x);
                            }
                        }
                    }
                }

                query = query.Where(predicate2);
            }

            var predicate = PredicateBuilder.BaseOr<Request>();
            predicate = predicate.Or(r => r.RequestHistoryLogs.Count(h => h.CreatedBy == CurrentUser.Id) > 0);
            predicate = predicate.Or(subpredicate);
            predicate = predicate.And(r => r.IsDeleted == false && (r.RequestRevisionId == null || r.RequestRevisionId == 0));
            if (!Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                predicate = predicate.Or(r => r.RewardRequest != null && r.RewardRequest.UserId == CurrentUser.Id);
            }
            else
            {
                predicate = predicate.And(r => r.CreatedBy == CurrentUser.Id && r.Type != (byte)RequestType.RefereeReward);
            }
            if (Utils.HasRolesAny(ArfUserRole.Referee))
            {
                predicate = predicate.And(r => r.CreatedBy != CurrentUser.Id || r.Type == (byte)RequestType.RefereeReward);
            }
            query = query.Where(predicate);

            var count = query.Count();

            if (filter != null)
            {
                var FilterPredicate = PredicateBuilder.BaseAnd<Request>();
                var CollegePredicate = PredicateBuilder.BaseOr<Request>();
                var RequestTypePredicate = PredicateBuilder.BaseOr<Request>();
                var StatusPredicate = PredicateBuilder.BaseOr<Request>();
                var FieldPredicate = PredicateBuilder.BaseOr<Request>();
                if (filter.Field != null)
                {
                    if (filter.Field.Count(x => x != null && x != -1) > 0)
                    {
                        foreach (var f in filter.Field)
                        {
                            if (f != null)
                            {
                                FieldPredicate = FieldPredicate.Or(x => x.FieldType == f);
                            }
                        }

                        FilterPredicate = FilterPredicate.And(FieldPredicate);
                    }
                }
                if (filter.College != null)
                {
                    if (filter.College.Count(x => x != null && x != -1) > 0)
                    {
                        foreach (var c in filter.College)
                        {
                            if (c != null)
                            {
                                CollegePredicate = CollegePredicate.Or(x => x.CollegeId == c);
                            }
                        }

                        FilterPredicate = FilterPredicate.And(CollegePredicate);
                    }
                }
                if (filter.Requesttype != null)
                {
                    if (filter.Requesttype.Count(x => x != null && x != -1) > 0)
                    {
                        foreach (var r in filter.Requesttype)
                        {
                            if (r != null)
                            {
                                RequestTypePredicate = RequestTypePredicate.Or(x => x.Type == r);
                            }
                        }
                        FilterPredicate = FilterPredicate.And(RequestTypePredicate);
                    }
                }
                if (filter.Statustype != null)
                {
                    if (filter.Statustype.Count(x => x != null && x != -1) > 0)
                    {
                        foreach (var s in filter.Statustype)
                        {
                            if (s != null)
                            {
                                StatusPredicate = StatusPredicate.Or(x => x.Status == s);
                            }
                        }
                        FilterPredicate = FilterPredicate.And(StatusPredicate);
                    }
                }                
                if (filter.Enddate != null)
                {
                    var date = Utils.ConvertDate(false, true, "yyyy-MM-dd", filter.Enddate.Value.ToString("yyyy-MM-dd"));
                    FilterPredicate = FilterPredicate.And(x => DbFunctions.TruncateTime(x.Created) <= DbFunctions.TruncateTime(date));
                }
                if (filter.Startdate != null)
                {
                    var date = Utils.ConvertDate(false, true, "yyyy-MM-dd", filter.Startdate.Value.ToString("yyyy-MM-dd"));
                    //var date = filter.Startdate.Value;
                    FilterPredicate = FilterPredicate.And(x =>DbFunctions.TruncateTime(x.Created) >= DbFunctions.TruncateTime(date));
                }
                if (!string.IsNullOrEmpty(filter.Researcher))
                {                    
                    FilterPredicate = FilterPredicate.And(x =>x.RequestCoresearchers.Count(y =>((string.IsNullOrEmpty(y.Researcher.FirstName)?"":y.Researcher.FirstName) +
                    (string.IsNullOrEmpty(y.Researcher.SecondName) ? "" : y.Researcher.SecondName) +
                    (string.IsNullOrEmpty(y.Researcher.ThirdName) ? "" : y.Researcher.ThirdName) +
                    (string.IsNullOrEmpty(y.Researcher.LastName) ? "" : y.Researcher.LastName)).ToLower().Trim().Contains(filter.Researcher.ToLower().Replace(" ", ""))) > 0);
                }
                if (!string.IsNullOrEmpty(filter.Title))
                {
                    FilterPredicate = FilterPredicate.And(z => z.TitleEnglish.ToLower() == filter.Title.ToLower() || z.Title == filter.Title);
                }
                query = query.Where(FilterPredicate);

            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (string.IsNullOrWhiteSpace(sortDir))
                    sortDir = "desc";
                else
                    sortDir = sortDir.ToLower().Trim();

                switch (sortBy.Trim().ToUpper())
                {
                    case "ID":
                        query = sortDir == "asc" ? query.OrderBy(r => r.Id) : query.OrderByDescending(r => r.Id);
                        break;

                    case "UNIQUEID":
                        query = sortDir == "asc" ? query.OrderBy(r => r.UniqueId) : query.OrderByDescending(r => r.UniqueId);
                        break;

                    case "TITLE":
                        query = sortDir == "asc" ? query.OrderBy(r => r.Title).ThenBy(r => r.PublicationRequest.Title) : query.OrderByDescending(r => r.PublicationRequest.Title).ThenByDescending(r => r.Title);
                        break;

                    case "COLLEGENAME":
                        var isArabic = Utils.IsUserLanguageArabic;

                        query = sortDir == "asc" ? query.OrderBy(r => !IsArabic ? r.College.NameEnglish : r.College.NameArabic) : query.OrderByDescending(r => !IsArabic ? r.College.NameEnglish : r.College.NameArabic);
                        break;

                    case "NAME":
                        query = sortDir == "asc" ? query.OrderBy(r => r.CreatedByUser.FullName) : query.OrderByDescending(r => r.CreatedByUser.FullName);
                        break;

                    case "TYPE":
                        query = sortDir == "asc" ? query.OrderBy(r => r.Type) : query.OrderByDescending(r => r.Type);
                        break;

                    case "DATECREATED":
                    case "DATECREATEDACTUAL":
                        query = sortDir == "asc" ? query.OrderBy(r => r.Created) : query.OrderByDescending(r => r.Created);
                        break;

                    case "STATUS":
                        query = sortDir == "asc" ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status);
                        break;

                    case "DATEUPDATED":
                    case "DATEUPDATEDACTUAL":
                    default:
                        query = sortDir == "asc" ? query.OrderBy(r => r.Updated) : query.OrderByDescending(r => r.Updated);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(r => r.Updated);
            }

            total = query.LongCount();

            if (pageSize >= 1 && pageNo >= 1)
            {
                query = query.Skip((pageNo.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        public string GenerateUniqueId(long? requestId, long announcementId, int collegeId)
        {
            var db = Utils.db;
            return GenerateUniqueId(requestId, db.Announcements.FirstOrDefault(a => a.Id == announcementId), db.Colleges.FirstOrDefault(a => a.Id == collegeId));
        }

        public string GenerateUniqueId(long? requestId, Announcement ann, College college)
        {
            if (college == null) return null;

            if (ann == null)
            {
                ann = Utils.GetActiveAnnouncement();
            }


            var startStr = ann.EndDateGreg.ToString("yy", CultureInfo.GetCultureInfo("en-US")) + "-" + ((ann.EndDateGreg.Year - 2018) + Constants.FundingPeriodFor2018) + "-" + (college.ResearchUnitCode + "").PadLeft(2, '0') + "-";


            var db = Utils.db;

            var lastRequest = db.Requests
                .Where(r => r.UniqueId.StartsWith(startStr))
                .Where(r => requestId == null || r.Id != requestId.Value)
                .OrderByDescending(r => r.UniqueId)
                .FirstOrDefault();

            if (lastRequest != null)
            {
                var num = int.Parse(lastRequest.UniqueId.Substring(startStr.Length));


                return startStr + ((num + 1) + "").PadLeft(3, '0');
            }
            var ss = startStr + ((1) + "").PadLeft(3, '0');
            return startStr + ((1) + "").PadLeft(3, '0');
        }

        public List<RequestHistoryModel> GetRequestHistory(Request request)
        {
            if (request == null || request.Created == null) return new List<RequestHistoryModel>();

            var history = request.RequestHistoryLogs.OrderByDescending(l => l.Created).ToList();

            var newHistory = new List<RequestHistoryLog>();

            var lastLog = null as RequestHistoryLog;

            for (var i = 0; i < history.Count; i++)
            {
                if (lastLog == null || (lastLog.CreatedBy != history[i].CreatedBy || lastLog.Type != history[i].Type || lastLog.Comments != history[i].Comments))
                {
                    newHistory.Add(history[i]);
                }

                lastLog = history[i];
            }

            return newHistory.Where(l => l.Type != null).Select(l => new RequestHistoryModel
            {
                Message = HuminizeHistoryLog(l),
                Type = (RequestHistoryTypes)l.Type,
                Date = l.Created,
                UserID = l.CreatedBy,
                Username = l.CreatedByUser.UserName,
                UserFullname = l.CreatedByUser.UserProfile != null ? l.CreatedByUser.UserProfile.GetFullName() : l.CreatedByUser.UserName
            }).OrderByDescending(h => h.Date).ThenByDescending(h => (byte)h.Type).ToList();
        }

        public string HuminizeHistoryLog(RequestHistoryLog l)
        {
            if (l == null || l.Type == null) return null;

            var msg = "";

            switch ((RequestHistoryTypes)l.Type)
            {
                default:
                    msg = ((RequestHistoryTypes)l.Type).ToString();
                    break;
            }

            if (msg == "") return null;

            if (!string.IsNullOrWhiteSpace(l.Comments))
            {
                msg += ".<br/>Comments: <b>" + l.Comments.Trim() + "</b>";
            }

            return msg;
        }

        public void SendNotification(Request request, TemplateKey key, ArfUserRole[] roles, ArfUserRole[] ccRoles, string[] userIds, string[] ccUserIds, string[] files, int collegeId = 0)
        {
            var template = TemplateUtils[key];

            var isFiltered = false;
            var predicate = PredicateBuilder.BaseOr<User>();
            var predicateCC = PredicateBuilder.BaseOr<User>();

            if (roles != null && roles.Length > 0)
            {
                foreach (var r in roles)
                {
                    if (r == ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch && collegeId != 0)
                    {
                        predicate = predicate.Or(u => u.Roles.Count(x => x.Name == r.ToString()) > 0 && u.CollegeId == collegeId);
                    }
                    else
                    {
                        predicate = predicate.Or(u => u.Roles.Count(x => x.Name == r.ToString()) > 0);
                    }
                }

                isFiltered = true;
            }

            if (ccRoles != null && ccRoles.Length > 0)
            {
                foreach (var r in ccRoles)
                {
                    predicateCC = predicateCC.Or(u => u.Roles.Count(x => x.Name == r.ToString()) > 0);
                }
            }

            if (userIds != null && userIds.Length > 0)
            {
                foreach (var u in userIds)
                {
                    predicate = predicate.Or(x => x.Id == u);
                }

                isFiltered = true;
            }

            if (ccUserIds != null && ccUserIds.Length > 0)
            {
                foreach (var u in ccUserIds)
                {
                    predicateCC = predicateCC.Or(x => x.Id == u);
                }
            }

            if (!isFiltered) return;

            var users = Utils.db.Users
                .Include("UserProfile")
                .Where(predicate)
                .ToList();

            var usersCC = Utils.db.Users
                .Include("UserProfile")
                .Where(predicateCC)
                .ToList();

            foreach (var c in users.Select(u => u.CollegeId).Distinct())
            {
                var collegeUsers = users.Where(u => u.CollegeId == c).ToList();
                var collegeUsersCC = users.Where(u => u.CollegeId == c).ToList();

                var cc = new List<MailAddress>();
                foreach (var u in collegeUsersCC)
                {
                    cc.Add(new MailAddress(u.Email, u.FullName));
                }

                foreach (var u in collegeUsers)
                {
                    var name = u.UserProfile != null ? u.UserProfile.GetFullName() : u.UserName;
                    var subject = template.SubjectArabic;
                    var body = template.BodyArabic.Replace("{name}", name);

                    if (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"))
                    {
                        EmailService.SendMessage(Utils.GetSmtpSettings(),
                            subject,
                            body,
                            new System.Net.Mail.MailAddress(u.Email, name),
                            cc.ToArray(),

                            files
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(u.PhoneNumber))
                    {
                        var smsBody = Utils.StripHTML(body);
                        SmsService.SendMessage(smsBody, new string[] { u.PhoneNumber });
                    }
                }
            }
        }

        public void SendNotification(Request request, TemplateKey SMSKey, TemplateKey EmailKey, ArfUserRole[] roles, ArfUserRole[] ccRoles, string[] userIds, string[] ccUserIds, string[] files)
        {
            var SMSTemplate = TemplateUtils[SMSKey];
            var EmailTemplate = TemplateUtils[EmailKey];

            var isFiltered = false;
            var predicate = PredicateBuilder.BaseOr<User>();
            var predicateCC = PredicateBuilder.BaseOr<User>();

            if (roles != null && roles.Length > 0)
            {
                foreach (var r in roles)
                {
                    predicate = predicate.Or(u => u.Roles.Count(x => x.Name == r.ToString()) > 0);
                }

                isFiltered = true;
            }

            if (ccRoles != null && ccRoles.Length > 0)
            {
                foreach (var r in ccRoles)
                {
                    predicateCC = predicateCC.Or(u => u.Roles.Count(x => x.Name == r.ToString()) > 0);
                }
            }

            if (userIds != null && userIds.Length > 0)
            {
                foreach (var u in userIds)
                {
                    predicate = predicate.Or(x => x.Id == u);
                }

                isFiltered = true;
            }

            if (ccUserIds != null && ccUserIds.Length > 0)
            {
                foreach (var u in ccUserIds)
                {
                    predicateCC = predicateCC.Or(x => x.Id == u);
                }
            }

            if (!isFiltered) return;

            var users = Utils.db.Users
                .Include("UserProfile")
                .Where(predicate)
                .ToList();

            var usersCC = Utils.db.Users
                .Include("UserProfile")
                .Where(predicateCC)
                .ToList();

            foreach (var c in users.Select(u => u.CollegeId).Distinct())
            {
                var collegeUsers = users.Where(u => u.CollegeId == c).ToList();
                var collegeUsersCC = users.Where(u => u.CollegeId == c).ToList();

                var cc = new List<MailAddress>();
                foreach (var u in collegeUsersCC)
                {
                    cc.Add(new MailAddress(u.Email, u.FullName));
                }

                foreach (var u in collegeUsers)
                {
                    var name = u.UserProfile != null ? u.UserProfile.GetFullName() : u.UserName;

                    if (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"))
                    {
                        var subject = EmailTemplate.SubjectArabic;
                        var body = EmailTemplate.BodyArabic.Replace("{FullName}", name);

                        EmailService.SendMessage(Utils.GetSmtpSettings(),
                            subject,
                            body,
                            new System.Net.Mail.MailAddress(u.Email, name),
                            cc.ToArray(),

                            files
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(u.PhoneNumber))
                    {
                        var subject = SMSTemplate.SubjectArabic;
                        var body = SMSTemplate.BodyArabic.Replace("{FullName}", name);
                        var smsBody = Utils.StripHTML(body);
                        SmsService.SendMessage(smsBody, new string[] { u.PhoneNumber });
                    }
                }
            }
        }

        public List<Data.RequestAction> GetRequestActions(Request request)
        {
            var workflow = (IRequestWorkflow)null;

            switch ((Data.RequestType)request.Type)
            {
                case Data.RequestType.FundingRequest:
                    workflow = new FundingRequestWorkflow();
                    break;

                case Data.RequestType.DesignRequest:
                    workflow = new DesignRequestWorkflow();
                    break;

                case Data.RequestType.MidTermProgressReport:
                    workflow = new MidTermRequestWorkflow();
                    break;

                case Data.RequestType.FulltermProgressReport:
                    workflow = new FinalTermRequestWorkflow();
                    break;

                case Data.RequestType.AdvertisementLetterForCollegeDeans:
                case Data.RequestType.AdvertisementLetterForDARWebsite:
                case Data.RequestType.AdvertisementLetterForDesigner:
                case Data.RequestType.AdvertisementLetterForIT:
                case Data.RequestType.AdvertisementLetterForNewspaper:
                    workflow = new LetterRequestWorkflow();
                    break;

                case Data.RequestType.MaterialRequest:
                    workflow = new MaterialRequestWorkflow();
                    break;

                case Data.RequestType.EditFundingRequest:
                    workflow = new EditFundingRequestWorkflow();
                    break;

                case Data.RequestType.RefereeReward:
                    workflow = new RefereeRewardWorkflow();
                    break;

                case Data.RequestType.AnnouncementRequest:
                    break;
            }

            if (workflow == null) return new List<Data.RequestAction>();
            return workflow.GetAllowedActions(request);
        }

        public List<AttachedFile> GetFiles(Request request, bool onlyLatest = false)
        {
            if (request == null || !(request.Id > 0)) return new List<AttachedFile>();

            var list = new List<AttachedFile>();


            list = request.RequestFiles.Where(rf => !((FileType)rf.Type).ToString().EndsWith("Opinion")).Select(c => new AttachedFile
            {
                FileId = c.Id.ToString(),
                Type = (FileType)c.Type,
                FileName = Path.GetFileName(c.FileName),
                Date = c.Created,
                FilePath = c.FilePath
            }).ToList();


            if (((Data.RequestType)request.Type).ToString().StartsWith("Advertisement"))
            {
                var arabicDesignFile = DbContext.RequestFiles
                    .Where(f => f.Request.AnnouncementId == request.AnnouncementId && f.Request.Type == (byte)Data.RequestType.DesignRequest)
                    .Where(f => f.Type == (byte)FileType.ArabicDesign)
                    .OrderByDescending(rf => rf.Created)
                    .Skip(0)
                    .Take(1)
                    .ToList()
                    .Select(c => new AttachedFile
                    {
                        FileId = c.Id.ToString(),
                        Type = (FileType)c.Type,
                        FileName = Path.GetFileName(c.FileName),
                        Date = c.Created,
                        FilePath = c.FilePath
                    }).FirstOrDefault();

                var englishDesignFile = DbContext.RequestFiles
                    .Where(f => f.Request.AnnouncementId == request.AnnouncementId && f.Request.Type == (byte)Data.RequestType.DesignRequest)
                    .Where(f => f.Type == (byte)FileType.EnglishDesign)
                    .OrderByDescending(rf => rf.Created)
                    .Skip(0)
                    .Take(1)
                    .ToList()
                    .Select(c => new AttachedFile
                    {
                        FileId = c.Id.ToString(),
                        Type = (FileType)c.Type,
                        FileName = Path.GetFileName(c.FileName),
                        Date = c.Created,
                        FilePath = c.FilePath
                    }).FirstOrDefault();

                if (englishDesignFile != null)
                    list.Insert(0, englishDesignFile);

                if (arabicDesignFile != null)
                    list.Insert(0, arabicDesignFile);
            }


            if (onlyLatest)
            {
                list = list.OrderByDescending(f => f.Date).ToList();

                var tmpList = new List<AttachedFile>();

                foreach (var f in list)
                {
                    if (tmpList.Count(x => x.Type == f.Type && x.Type > 0) > 0) continue;

                    tmpList.Add(f);
                }

                return tmpList;
            }



            return list;

        }

        public List<FileType> RequiredFiles(Request request)
        {
            if (request == null || !(request.Id > 0)) return new List<FileType>();

            var list = new List<FileType>();

            if (request.Type == (byte)Data.RequestType.DesignRequest && (request.Status == (long)RequestStatus.DesignRequestReceivedByDesigner || request.Status == (long)RequestStatus.DesignRequestReceivedByDesignerAfterDesigner) && Utils.HasRolesAny(ArfUserRole.Designer))
            {
                list.Add(FileType.ArabicDesign);
                list.Add(FileType.EnglishDesign);
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForNewspaper && request.Status == (long)RequestStatus.DeansLetterRequestPendingSubmission && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
            {
                list.Add(FileType.NewspaperLetter);
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForCollegeDeans && request.Status == (long)RequestStatus.DeansLetterRequestPendingSubmission && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
            {
                list.Add(FileType.CollegeDeansLetter);
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForIT && request.Status == (long)RequestStatus.DeansLetterRequestPendingSubmission && Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
            {
                list.Add(FileType.ItLetter);
            }
            else if (request.Type == (byte)Data.RequestType.FundingRequest && (request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcherForUploadQuotations || request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcher) && Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
            {
                var materialCost = request.RequestMaterials.Sum(x => x.Cost);
                if (materialCost > 0)
                {
                    list.Add(FileType.RequestMaterialsQuotation1);
                    list.Add(FileType.RequestMaterialsQuotation2);
                    list.Add(FileType.RequestMaterialsQuotation3);
                }
            }
            else if (request.Type == (byte)Data.RequestType.MaterialRequest && request.Status == (long)RequestStatus.MaterialRequestReceivedByDirectorGeneralOfAdministrationAndFinance && Utils.HasRolesAny(ArfUserRole.DirectorGeneralOfAdministrationAndFinance))
            {
                list.Add(FileType.MaterialsRequestCheque);
            }
            return list;
        }

        public List<FileType> MissingFiles(Request request, List<AttachedFile> Files)
        {
            if (request == null || !(request.Id > 0)) return new List<FileType>();
            if (Files == null) Files = new List<AttachedFile>();

            var list = new List<FileType>();
            var requiredFiles = RequiredFiles(request);


            foreach (var file in requiredFiles)
            {
                if (request.RequestFiles.Count(f => f.Type == (byte)file) == 0 && Files.Count(f => f.Type == file && f.Content != null && f.Content.ContentLength > 0) == 0)
                {
                    list.Add(file);
                }
            }



            return list;
        }
    }
}