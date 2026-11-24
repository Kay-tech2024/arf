using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class MidTermRequestWorkflow : IRequestWorkflow
    {
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.MidTermProgressReport)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
            
            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
            
            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.MidTermReportPendingSubmission:
                        if (Utils.HasRolesAny(ArfUserRole.PrincipalResearcher))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToViceDeanForHigherStudiesAndScientificResearch,
                                RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSelectReferees,
                                RequestStatus.MidTermReportReceivedByReferee,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MidTermReportReceivedByReferee:
                        if (Utils.HasRolesAny(ArfUserRole.Referee))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearchAfterReferee,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.Reject,
                               RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearchAfterReferee,
                               null
                           ));
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ApproveWithComments,
                               RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearchAfterReferee,
                               null
                           ));

                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;
                    case RequestStatus.MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearchAfterReferee:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToResearcher,
                                RequestStatus.MidTermReportRecievedByResearcher,
                                null
                            ));
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SelectReferees,
                                RequestStatus.MidTermReportReceivedByReferee,
                                null
                            ));
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSendToDirectorOfFinancing,
                                RequestStatus.MidTermReportRecievedByDirector_of_Research_Projects_Financing_Program,
                                null
                            ));
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.RejectAndReturnToDirectorOfFinancingProgram,
                                RequestStatus.MidTermReportRecievedByDirector_of_Research_Projects_Financing_Program,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.MidTermReportRecievedByDirector_of_Research_Projects_Financing_Program:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorGeneralOfAdministrationAndFinance) && request.PublicationRequest.CreatedBy == Utils.CurrentUser.Id)
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndForwardToViceDeanOfResearchCentersAndDatabases,
                                RequestStatus.MidTermReportRecievedByVice_of_Deanship_Research_Centers_and_databases,
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
                    case RequestStatus.MidTermReportRecievedByVice_of_Deanship_Research_Centers_and_databases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDeanOfScientificResearch,
                                RequestStatus.MidTermReportRecievedByDeanOfScientificResearch,
                                null
                            ));

                        }
                        
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;                   
                    case RequestStatus.MidTermReportRecievedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndForwardToAccountant,
                                RequestStatus.MidTermReportRecievedByAccountant_of_Deanship_of_Scientific_Research,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;



                    case RequestStatus.MidTermReportRecievedByAccountant_of_Deanship_of_Scientific_Research:
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
