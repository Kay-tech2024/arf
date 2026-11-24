using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class EditFundingRequestWorkflow : IRequestWorkflow
    {

        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.EditFundingRequest)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.EditFundingRequestPendingSubmissionByResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch,
                                RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch:
                    case RequestStatus.EditFundingRequestPendingCollegeCommittee:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDeanOfScientificResearch,
                                RequestStatus.EditFundingRequestReceivedByDeanOfScientificResearch,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.SendToResearcher,
                               RequestStatus.EditFundingRequestPendingSubmissionByResearcher,
                               null));

                            if (st != RequestStatus.EditFundingRequestPendingCollegeCommittee)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.SendToCommiteeMembers,
                               RequestStatus.EditFundingRequestPendingCollegeCommittee,
                               null));
                            }
                        }
                        else if (st == RequestStatus.EditFundingRequestPendingCollegeCommittee && Utils.HasRolesAny(ArfUserRole.ResearchUnitMember) && request.CollegeId == Utils.UserUtils.CurrentUser.CollegeId && (request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id) == 0 || request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id && rr.Action == null) > 0))
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Approve,
                                RequestStatus.EditFundingRequestPendingCollegeCommittee, null));
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Reject,
                                RequestStatus.EditFundingRequestPendingCollegeCommittee, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.EditFundingRequestReceivedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.EditFundingRequestReceivedByResearchCentersAndDatabases,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.EditFundingRequestReceivedByResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.EditFundingRequestReceivedByDirectorOfProgramOfFinance,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.EditFundingRequestReceivedByDirectorOfProgramOfFinance:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects,
                               RequestStatus.EditFundingRequestReceivedBySecretaryOfStandingCommitteeForFinancingResearchProjects,
                               null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.EditFundingRequestReceivedBySecretaryOfStandingCommitteeForFinancingResearchProjects:
                        if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToCommiteeMembers,
                                RequestStatus.EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects:
                        if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.EditFundingRequestReceivedByViceDeanForResearchCentersAndDatabases,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null));
                        }
                        else if (Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects) && (request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id) == 0 || request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id && rr.Action == null) > 0))
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                 RequestAction.Approve,
                                 RequestStatus.EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects, null));

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Reject,
                                RequestStatus.EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.EditFundingRequestReceivedByViceDeanForResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.Approve,
                               RequestStatus.EditFundingRequestReceivedByDirectorOfProgramOfFinanceAfterViceDeanForResearchCentersAndDatabasesDecision,
                               null
                           ));

                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.EditFundingRequestReceivedByDirectorOfProgramOfFinanceAfterViceDeanForResearchCentersAndDatabasesDecision:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.Approve,
                               RequestStatus.Completed,
                               null
                           ));

                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                }
            }

            return list;

        }

        public RequestStatus[] GetRequestStatus(Request request)
        {
            return new RequestStatus[] { (RequestStatus)request.Status };
        }

        public void OnCompleted(Request request)
        {

        }
    }

}
