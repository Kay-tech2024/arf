using ARF.Data;
using ARF.Data.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class FundingRequestWorkflow : IRequestWorkflow
    {
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.FundingRequest)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.FundingRequestPendingSubmission:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            if (request.RequestCoresearchers.Count(r => r.Researcher.UserId != request.CreatedBy && r.DidResearcherAgree == null && r.IsDeleted == false) > 0)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                   Data.RequestAction.SendToCoresearchers,
                                   RequestStatus.FundingRequestPendingCoresearchers,
                                   null
                               ));
                            }
                            else
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                   Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch,
                                   RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies,
                                   null
                               ));
                            }

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Cancel,
                                RequestStatus.Canceled, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FundingRequestPendingCoresearchers:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.RequestCoresearchers.Count(cr => cr.ResearcherId == Utils.ResearcherId && cr.DidResearcherAgree == null && cr.IsDeleted == false) > 0)
                        {
                            if (request.RequestCoresearchers.Count(r => r.Researcher.UserId != request.CreatedBy && r.DidResearcherAgree == null && r.IsDeleted == false) > 0)
                                list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Accept,
                                RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies, null));
                            else
                                list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Accept,
                                RequestStatus.FundingRequestPendingCoresearchers, null));

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.RejectAndReturnToResearcher,
                                RequestStatus.FundingRequestRejectedByCoresearchers, new string[] { request.CreatedBy }));
                        }
                        else
                        {
                            return new List<Tuple<RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestRejectedByCoresearchers:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.ResubmitAndReturnToCoresearchers,
                                RequestStatus.FundingRequestPendingCoresearchers, null));

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Cancel,
                                RequestStatus.Canceled, null));
                        }
                        else
                        {
                            return new List<Tuple<RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReturnedToResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch,
                                RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FundingRequestPendingModificationsByResearcher:
                    case RequestStatus.FundingRequestPendingModificationsByResearcherForUploadQuotations:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            if (request.Status == (long)RequestStatus.FundingRequestPendingModificationsByResearcherForUploadQuotations)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                        Data.RequestAction.SendToDirectorOfProgramOfFinance, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcherUploadQuotations,
                                        null
                                 ));
                            }
                            else
                            {
                                var reqVersionCount = Utils.db.Requests.Where(x => x.RequestRevisionId == request.Id).Count();
                                if (reqVersionCount > 0 && request.RequestCoresearchers.Where(x => x.IsDeleted == false && x.ResearcherType != (byte)ResearcherRole.MainResearcher && x.ResearcherType != (byte)ResearcherRole.AssistantResearcher).Count() > 0)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                        Data.RequestAction.SendToCoresearchers, RequestStatus.FundingRequestPendingCoresearchersAfterRequestUpdateAfterRefereeDecision,
                                        null
                                    ));
                                }
                                else
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                        Data.RequestAction.SendToDirectorOfProgramOfFinance, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher,
                                        null
                                    ));
                                }
                            }
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FundingRequestRecievedByViceDeanOfGraduateStudies:
                    case RequestStatus.FundingRequestPendingCollegeCommittee:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && request.CollegeId == Utils.CurrentUser.CollegeId)
                        {
                            if (st != RequestStatus.FundingRequestPendingCollegeCommittee)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.SendToCommiteeMembers,
                                    RequestStatus.FundingRequestPendingCollegeCommittee, null));
                            }

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDeanshOfScientificResearch,
                                RequestStatus.FundingRequestRecievedByDeanOfScientificResearch, null));
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ReturnToResearcher,
                               RequestStatus.FundingRequestReturnedToResearcher, null));
                        }
                        else if (st == RequestStatus.FundingRequestPendingCollegeCommittee && Utils.HasRolesAny(ArfUserRole.ResearchUnitMember) && request.CollegeId == Utils.UserUtils.CurrentUser.CollegeId && (request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id) == 0 || request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id && rr.Action == null) > 0))
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Approve,
                                RequestStatus.FundingRequestPendingCollegeCommittee, null));
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Reject,
                                RequestStatus.FundingRequestPendingCollegeCommittee, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FundingRequestRecievedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToViceDean_for_research_centers_and_databases,
                                RequestStatus.FundingRequestRecivedByViceDeanOfResearchCenterAndDatabases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestRecivedByViceDeanOfResearchCenterAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancing,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestRecievedBySecertaryOfDeanShipResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheDeanshipOfResearchCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSendToDirectorOfFinancing,
                                RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancing,
                                null)
                            );

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.RejectAndReturnToResearcher,
                                RequestStatus.FundingRequestPendingModificationsByResearcher,
                                null)
                            );
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancing:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects,
                                RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinance,
                                null)
                            );

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ReturnToResearcher,
                               RequestStatus.FundingRequestReceivedByResearcher,
                               null)
                           );
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedByResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancing,
                                null)
                            );
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinance:
                    case RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinanceAfterDirectorOfProgramOfFinancing:
                    case RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects:
                    case RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing:
                        if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                        {

                            if (request.Status == (long)RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinanceAfterDirectorOfProgramOfFinancing || request.Status == (long)RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.ApproveAndSendToDirectorOfFinancing,
                                    RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterSecretary,
                                    null
                                ));
                            }

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSelectReferees,
                                RequestStatus.FundingRequestReceivedByViceDeanOfResearchCentersAndDatabasesAfterRefereeSelection,
                                null
                            ));

                            if (st == RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinance)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.SendToCommiteeMembers,
                                    RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects,
                                    null
                                ));
                            }
                            else if (st == RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinanceAfterDirectorOfProgramOfFinancing)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.SendToCommiteeMembers,
                                    RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing,
                                    null
                                ));
                            }
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null
                            ));

                        }
                        else if (st == RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects && Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects) && (request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id) == 0 || request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id && rr.Action == null) > 0))
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                 RequestAction.Approve,
                                 RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects, null));

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Reject,
                                RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects, null));
                        }
                        else if (st == RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing && Utils.HasRolesAny(ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects) && (request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id) == 0 || request.RequestUsers.Count(rr => rr.UserId == Utils.CurrentUser.Id && rr.Action == null) > 0))
                        {
                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                 RequestAction.Approve,
                                 RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing, null));

                            list.Add(new Tuple<RequestAction, RequestStatus, string[]>(
                                RequestAction.Reject,
                                RequestStatus.FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedByViceDeanOfResearchCentersAndDatabasesAfterRefereeSelection:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeSelection,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeSelection:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToMainReferees,
                                RequestStatus.FundingRequestPendingMainRefrees,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestPendingMainRefrees:
                        if (Utils.HasRolesAny(ArfUserRole.Referee) && request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.RoundNo == 2) > 0)
                        {
                            var totalAccepted = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Approve);
                            var totalAcceptedWithComments = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.ApproveWithComments);
                            var totalRejected = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Reject);
                            var totalRejectToReview = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.RejectToReview);

                            if (request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.Action == null && rr.DidRefereeAgree == null) > 0)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.AcceptToReview, RequestStatus.FundingRequestPendingMainRefrees, null));
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.RejectToReview, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                            }
                            else if (request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.SubmittedOn == null) > 0)
                            {
                                if (totalAccepted >= 1 || totalAcceptedWithComments >= 1)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                }
                                else if (totalRejected >= 1)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                }
                                else
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestPendingMainRefrees, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestPendingMainRefrees, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestPendingMainRefrees, null));
                                }
                            }
                            else
                            {
                                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                            }
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedBySpareReferee:
                        if (Utils.HasRolesAny(ArfUserRole.Referee) && request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.RequestReviewSentOn != null) > 0)
                        {
                            if (request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.Action == null && rr.DidRefereeAgree == null) > 0)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.AcceptToReview, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.RejectToReview, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                            }
                            else if (request.RequestReferees.Count(rr => rr.Referee.UserId == Utils.CurrentUser.Id && rr.SubmittedOn == null) > 0)
                            {
                                var totalAccepted = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Approve);
                                var totalAcceptedWithComments = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.ApproveWithComments);
                                var totalRejected = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Reject);
                                if (totalAccepted >= 1 || totalAcceptedWithComments >= 1)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                    if (totalRejected >= 1)
                                    {
                                        list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                    }
                                    else
                                    {
                                        list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    }
                                    
                                }
                                else if (totalRejected >= 1)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision, null));
                                }
                                else
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Approve, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.ApproveWithComments, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(Data.RequestAction.Reject, RequestStatus.FundingRequestReceivedBySpareReferee, null));
                                }
                            }
                            else
                            {
                                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                            }
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;
                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            var totalResponses = request.RequestReferees.Count();
                            var totalAccepted = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Approve);
                            var totalAcceptedWithComments = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.ApproveWithComments);
                            var totalRejected = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.Reject);
                            var materialCost = request.RequestMaterials.Sum(x => x.Cost);

                            if (totalAccepted >= 2)
                            {
                                if (materialCost > 0)
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                        Data.RequestAction.ApproveAndReturnToResearcherToUploadQuotation,
                                        RequestStatus.FundingRequestPendingModificationsByResearcherForUploadQuotations,
                                        null
                                    ));
                                }
                                else
                                {
                                    list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                       Data.RequestAction.Approve,
                                       RequestStatus.FundingRequestWaitingForContractPreparation,
                                       null
                                   ));
                                }
                            }
                            else if (totalRejected >= 2)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                   Data.RequestAction.Reject,
                                   RequestStatus.Rejected,
                                   null
                               ));
                            }
                            else if (totalAcceptedWithComments >= 1 && (totalAccepted >= 1 || totalAcceptedWithComments >= 2))
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                   Data.RequestAction.ApproveAndReturnToResearcher,
                                   RequestStatus.FundingRequestPendingModificationsByResearcher,
                                   null
                               ));
                            }
                            else
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.ForwardToSpareReferee,
                                    RequestStatus.FundingRequestReceivedBySpareReferee,
                                    null
                                ));
                            }
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher:
                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcherUploadQuotations:
                    case RequestStatus.FundingRequestReceivedByDirectorOfProgramOfFinancingAfterSecretary:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            var totalAcceptedWithComments = request.RequestReferees.Count(rr => rr.Action == (byte)Data.RequestAction.ApproveWithComments);
                            var materialCost = request.RequestMaterials.Sum(x => x.Cost);
                            if (materialCost > 0)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                    Data.RequestAction.ApproveAndReturnToResearcherToUploadQuotation,
                                    RequestStatus.FundingRequestPendingModificationsByResearcherForUploadQuotations,
                                    null
                                ));
                            }
                            else if (totalAcceptedWithComments >= 1)
                            {
                                list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                   Data.RequestAction.ReturnToResearcherForModifications,
                                   RequestStatus.FundingRequestPendingModificationsByResearcher,
                                   null
                                ));
                            }

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects,
                               RequestStatus.FundingRequestRecievedBySecretaryOfCommitteeResearchFinanceAfterDirectorOfProgramOfFinancing,
                               null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.Approve,
                               RequestStatus.FundingRequestWaitingForContractPreparation,
                               null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.Reject,
                               RequestStatus.Rejected,
                               null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestWaitingForContractPreparation:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.FundingRequestContractReceivedByResearchCentersAndDatabases, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestContractReceivedByResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSendToMainResearcher,
                                RequestStatus.FundingRequestContractIssuedWaitingForResearcher, null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ReturnToDirectorOfFinancingProgram,
                                RequestStatus.FundingRequestWaitingForContractPreparation, null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                    case RequestStatus.FundingRequestContractIssuedWaitingForResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve, RequestStatus.FundingRequestContractReceivedByDeanOfScientificResearchAfterResearcher, null)
                            );

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject, RequestStatus.Rejected, null)
                            );
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FundingRequestContractReceivedByDeanOfScientificResearchAfterResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.Completed, null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject, RequestStatus.Rejected, null)
                            );
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }
                        break;

                }
            }
            return list.Distinct().ToList();
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