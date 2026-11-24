using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Managers
{
    public class RequestsManager : AbstractManager
    {
        public Request GetSingleRequest(long? id)
        {
            return RequestUtils.GetRequests(null).SingleOrDefault(c => c.Id == id);
        }


        public IQueryable<Request> GetRequests(long? CollegeId = null, long? groupId = null, string userId = null, RequestType? type = null, RequestTab? status = null, string queryStr = null, int? pageNo = null, int? pageSize = null)
        {
            long total = 0;
            return GetRequests(CollegeId, groupId, userId, type, status, queryStr, pageNo, pageSize, out total);
        }

        public IQueryable<Request> GetRequests(long? collegeId, long? CollegeId, string userId, RequestType? type, RequestTab? status, string queryStr, int? pageNo, int? pageSize, out long total)
        {
            total = 0;
            if (Roles == null || Roles.Length == 0)
                return new List<Request>().AsQueryable();



            var query = DbContext.Requests.AsQueryable();

            if (collegeId != null)
            {
                query = query.Where(r => r.CreatedByUser.CollegeId == collegeId);
            }

            if (CollegeId != null)
            {
                query = query.Where(r => r.CreatedByUser.CollegeId == CollegeId);
            }

            if (userId != null)
            {
                query = query.Where(r => r.CreatedBy == userId);
            }

            if (type != null)
            {
                query = query.Where(r => r.Type == (byte)type);
            }


            /*if (Utils.HasRolesAny(ArfUserRole.Researcher))
            {
                query = query.Where(r => r.CreatedBy == CurrentUser.Id || r.PublicationRequest.CreatedBy == CurrentUser.Id);
            }
            else if (Utils.HasRolesAny(ArfUserRole.Referee))
            {
                query = query.Where(r => r.RequestReferees.Count(rr => rr.Referee.UserId == CurrentUser.Id) > 0);
            }*/

            if (status != null)
            {

                switch (status.Value)
                {
                    case RequestTab.All:
                        {

                        }
                        break;
                    case RequestTab.Inbox:
                        {
                            switch (Utils.GetUserRoles().First())
                            {
                                /*case ArfUserRole.PrincipalResearcher:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status==(long)RequestStatus.PublishRequestNotSubmitted);
                                    break;
                                case ArfUserRole.VicePresidentForHigherStudiesAndScientificResearch:
                                    query = query.Where(r => r.Status == (long)RequestStatus.PublishRequestPendingResearchUnitSecretary);
                                    break;                                
                                case ArfUserRole.Designer:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.PublishRequestNotSubmitted);
                                    break;
                                case ArfUserRole.DeanofDeanshipofScientificResearch:
                                    query = query.Where(r => r.Status == (long)RequestStatus.PublishRequestNotSubmitted);
                                    break;
                                case ArfUserRole.DeanshipofResearchcCentersAndDatabases:
                                    query = query.Where(r => r.Status == (long)RequestStatus.PublishRequestNotSubmitted);
                                    break;
                                case ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers:
                                    query = query.Where(r => r.Status == (long)RequestStatus.PublishRequestNotSubmitted);
                                    break;*/

                                case ArfUserRole.VicePresidentForHigherStudiesAndScientificResearch:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedFundingRequestByViceDeanOfGraduateStudies);
                                    break;
                                case ArfUserRole.FacultyMember:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedFundingRequestByResearchUnitMember);
                                    break;
                                case ArfUserRole.DeanofDeanshipofScientificResearch:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedByDeanOfScientificResearch);
                                    break;
                                case ArfUserRole.DeanshipofResearchcCentersAndDatabases:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecivedByViceDeanOfResearchCenter);
                                    break;
                                case ArfUserRole.SecretaryOfTheDeanshipOfResearchCentersAndDatabases:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedBySecertaryOfDeanShip);
                                    break;
                                case ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedByDeanOfResearchCenter);
                                    break;
                                case ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedBySecretaryOfCommitteeResearchFinance);
                                    break;
                                case ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RecievedByMembersOfCommitteeResearchFinance);
                                    break;
                                case ArfUserRole.ThetwoMainArbitrators:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.ApprovedByOneRefrees);
                                    break;
                                case ArfUserRole.Arbitrator:
                                    query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.RejectedByRefrees);
                                    break;
                               //** case ArfUserRole.Arbitrator:
                               //     query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.ApprovedByThirdRefree);
                               //     break;
                               // ** case ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers:
                               //     query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.ContractSignedByDeanOfResearchCenter);
                               //     break;
                               // ** case ArfUserRole.DeanofDeanshipofScientificResearch:
                               //     query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.ContractSignedByDeanOfScientificResearch);
                               //     break;
                            }
                        }
                        var isResearcher = Utils.HasRolesAny(ArfUserRole.Researcher);
                        //query = query.Where(r => r.CurrentlyWith == CurrentUser.Id || (isResearcher && r.Status >= (long)RequestStatus.PublishRequestPendingResearchUnitSecretary));
                        break;
                    case RequestTab.Outbox:
                        query = query.Where(r => r.CreatedBy == CurrentUser.Id || r.RequestHistoryLogs.Count(h => h.CurrentlyWith == CurrentUser.Id) > 0);
                        break;
                    case RequestTab.Pending:
                        query = query.Where(r => r.CreatedBy == CurrentUser.Id && r.Status == (long)RequestStatus.Pending);
                        break;
                    case RequestTab.Overdue:
                        var overdueDate = DateTime.Now.AddDays(-7);
                        query = query.Where(r => r.Status != (long)RequestStatus.Pending && r.Created < overdueDate);
                        break;

                    case RequestTab.History:
                        query = query.Where(r => r.Status != (long)RequestStatus.Pending);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryStr = queryStr.Trim().ToLower();
                long t = 0;
                if (IsValidNationalId(queryStr))
                {
                    query = query.Where(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.NationalId == queryStr);
                }
                else if (long.TryParse(queryStr, out t))
                {
                    query = query.Where(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.EmployeeId.Contains(queryStr));
                }
                else
                {
                    var names = Utils.GetJobseeekerNameComponents(queryStr);
                    if (names != null && names.FirstName != null)
                    {
                        query = query.Where(r => r.CreatedByUser.UserProfile != null && r.CreatedByUser.UserProfile.FirstName == names.FirstName && (names.LastName == null || r.CreatedByUser.UserProfile.LastName == names.LastName));
                    }
                }
            }

            query = query.OrderByDescending(r => r.Id);

            total = query.LongCount();

            if (pageSize >= 1 && pageNo >= 1)
            {
                query = query.Skip((pageNo.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }


            return query;

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

        public static NameComponents GetNameComponents(string str)
        {

            if (string.IsNullOrWhiteSpace(str)) return null;
            if (str.StartsWith("د.")) str = str.Substring(2);
            var tabs = str.Trim().Split(' ').Where(s => s != "" && s != "بن" && s != "بنت").ToArray();
            if (tabs.Length > 1 && tabs[0] == "د.") tabs = tabs.Skip(1).ToArray();
            var components = new NameComponents
            {
                FirstName = tabs[0]
            };
            var startIndex = 1;
            if ((components.FirstName == "عبد" || Utils.NormalizeString(components.FirstName) == "ابو" || Utils.NormalizeString(components.FirstName) == "ابن") && tabs.Length > 1)
            {
                components.FirstName += " " + tabs[1];
                startIndex = 2;
            }
            else if (tabs.Length > 1 && tabs[1] == "الدين")
            {
                components.FirstName += " " + tabs[1];
                startIndex = 2;
            }
            if (startIndex < tabs.Length)
            {
                components.SecondName = tabs[startIndex];
                startIndex++;
                if (components.SecondName == "عبد" && tabs.Length > startIndex)
                {
                    components.SecondName += tabs[startIndex];
                    startIndex++;
                }
            }
            if (startIndex < tabs.Length)
            {
                components.ThirdName = tabs[startIndex];
                startIndex++;
                if (components.ThirdName == "عبد" && tabs.Length > startIndex)
                {
                    components.ThirdName += tabs[startIndex];
                    startIndex++;
                }
            }
            if (startIndex < tabs.Length)
            {
                components.LastName = tabs[startIndex];
                startIndex++;
                if (components.LastName == "عبد" || components.LastName == "ال" && tabs.Length > startIndex)
                {
                    components.LastName += tabs[startIndex];
                    startIndex++;
                }
            }
            if (components.LastName == null)
            {
                if (components.ThirdName != null)
                {
                    components.LastName = components.ThirdName;
                    components.ThirdName = null;
                }
                else if (components.SecondName != null)
                {
                    components.LastName = components.SecondName;
                    components.SecondName = null;
                }
            }
            else if (components.SecondName == "آل")
            {
                if (components.ThirdName != null)
                {
                    components.SecondName = "آل " + components.ThirdName;
                    components.ThirdName = null;
                }
                else if (components.LastName != null)
                {
                    components.LastName = "آل " + components.LastName;
                    components.SecondName = null;
                }
            }
            else if (components.ThirdName == "آل")
            {
                components.ThirdName = null;
                components.LastName = "آل " + components.LastName;
            }
            if ((components.FirstName.StartsWith("ال") || components.FirstName.StartsWith("ابن ال")) && components.SecondName == null && components.ThirdName == null && components.LastName == null)
            {
                components.LastName = components.FirstName;
                components.FirstName = null;
            }
            return components;
        }
    }
}