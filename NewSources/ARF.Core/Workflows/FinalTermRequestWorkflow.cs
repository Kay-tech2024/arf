using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class FinalTermRequestWorkflow : IRequestWorkflow
    {
        
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.FulltermProgressReport)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

          
            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.FinalReportPendingSubmission:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByResearcher:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                           
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.FinalReportRecievedByDirector_of_Research_Projects_Financing_Program,
                                null
                            ));


                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByDirector_of_Research_Projects_Financing_Program:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToDeanOfScientificResearch,
                                RequestStatus.FinalReportRecievedByDeanOfScientificResearch,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.MarkUnofficialAndForwardToDeanOfScientificResearch,
                                RequestStatus.FinalReportRecievedByDeanOfScientificResearchContainer,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndForwardToAccountant,
                                RequestStatus.FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByDeanOfScientificResearchContainer:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToViceDean_for_research_centers_and_databases,
                                RequestStatus.FinalReportRecievedBy_ViceDean_for_research_centers_and_databases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research:
                        if (Utils.HasRolesAny(ArfUserRole.Accountant))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.PaidClaims,
                                RequestStatus.Completed,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedBy_ViceDean_for_research_centers_and_databases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects,
                                RequestStatus.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects:
                        if (Utils.HasRolesAny(ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveContainer,
                                RequestStatus.FinalReportRecievedByResearcher,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.RejectContainer,
                               RequestStatus.FinalReportRecievedByResearcher,
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
