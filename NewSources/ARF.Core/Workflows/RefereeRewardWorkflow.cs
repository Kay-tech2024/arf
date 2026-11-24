using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class RefereeRewardWorkflow : IRequestWorkflow
    {
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.RefereeReward)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
            
            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

      
            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendTo_ViceDean_for_research_centers_and_databases,
                                RequestStatus.RefereeRewardRecievedBy_ViceDean_for_research_centers_and_databases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.RefereeRewardRecievedBy_ViceDean_for_research_centers_and_databases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDeanOfScientificResearch,
                                RequestStatus.RefereeRewardRecievedByDeanOfScientificResearch,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.RefereeRewardRecievedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndForwardToAccountant,
                                RequestStatus.RefereeRewardRecievedByAccountant_of_Deanship_of_Scientific_Research,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.RejectAndReturnToDirectorOfFinancingProgram,
                                RequestStatus.RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.RefereeRewardRecievedByAccountant_of_Deanship_of_Scientific_Research:
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
