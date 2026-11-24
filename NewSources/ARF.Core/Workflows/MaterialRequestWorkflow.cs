using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class MaterialRequestWorkflow : IRequestWorkflow
    {
        
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.MaterialRequest)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
            
            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.MaterialRequestPendingSubmissionByResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch,
                                RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.MaterialRequestReceivedByDirectorOfProgramOfFinance,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MaterialRequestReceivedByDirectorOfProgramOfFinance:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.MaterialRequestReceivedByResearchCentersAndDatabases,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MaterialRequestReceivedByResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDeanOfScientificResearch,
                                RequestStatus.MaterialRequestReceivedByDeanOfScientificResearch,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.MaterialRequestReceivedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.MaterialRequestReceivedByVicePresidentForHigherStudiesAndScientificResearch,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.MaterialRequestReceivedByVicePresidentForHigherStudiesAndScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.VicePresidentForHigherStudiesAndScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndForwardToUniversityDirector,
                                RequestStatus.MaterialRequestReceivedByUniversityDirector,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MaterialRequestReceivedByUniversityDirector:
                        if (Utils.HasRolesAny(ArfUserRole.UniversityDirector, ArfUserRole.UniversityRector))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSendToDirectorGeneralOfAdministrationAndFinance,
                                RequestStatus.MaterialRequestReceivedByDirectorGeneralOfAdministrationAndFinance,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Reject,
                                RequestStatus.Rejected,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    
                    case RequestStatus.MaterialRequestReceivedByDirectorGeneralOfAdministrationAndFinance:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorGeneralOfAdministrationAndFinance))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.SendToResearcher,
                               RequestStatus.MaterialRequestReceivedByResearcherAfterDirectorGeneralOfAdministrationAndFinance,
                               null
                           ));

                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.MaterialRequestReceivedByResearcherAfterDirectorGeneralOfAdministrationAndFinance:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Confirm,
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
