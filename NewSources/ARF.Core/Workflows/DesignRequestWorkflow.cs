using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class DesignRequestWorkflow : IRequestWorkflow
    {

        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public RequestStatus[] GetRequestStatus(Request request)
        {
            return new RequestStatus[] { (RequestStatus)request.Status };

        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null || request.Type != (byte)Data.RequestType.DesignRequest)
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinance:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDesigner,
                                RequestStatus.DesignRequestReceivedByDesigner,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DesignRequestReceivedByDesigner:
                        if (Utils.HasRolesAny(ArfUserRole.Designer))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinanceAfterDesign,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinanceAfterDesign:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ReturnToDesigner,
                                RequestStatus.DesignRequestReceivedByDesignerAfterDesigner,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.SendToViceDeanForResearchCentersAndDatabases,
                               RequestStatus.DesignRequestReceivedByViceDeanForResearchCentersAndDatabases,
                               null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DesignRequestReceivedByDesignerAfterDesigner:
                        if (Utils.HasRolesAny(ArfUserRole.Designer))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendToDirectorOfProgramOfFinance,
                                RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinanceAfterDesign,
                                null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;


                    case RequestStatus.DesignRequestReceivedByViceDeanForResearchCentersAndDatabases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ReturnToDirectorOfFinancingProgram,
                                RequestStatus.DesignRequestReceivedByDirectorOfProgramOfFinanceAfterDesign,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.SendToDeanOfScientificResearch,
                               RequestStatus.DesignRequestReceivedByDeanOfScientificResearch,
                               null));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DesignRequestReceivedByDeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.Completed,
                                null));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ReturnToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.DesignRequestReceivedByViceDeanForResearchCentersAndDatabases,
                                null));
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


        public void OnCompleted(Request request)
        {
            var db = Utils.db;
            var utils = Utils.RequestUtils;
            var currentUser = Utils.UserUtils.CurrentUser;

            var itLetterRequest = new Request
            {
                Title = request.Title,
                CollegeId = request.CollegeId,
                AnnouncementId = request.AnnouncementId,
                PublicationRequestId = request.PublicationRequestId,
                Status = (long)RequestStatus.DeansLetterRequestPendingSubmission,
                Type = (byte)Data.RequestType.AdvertisementLetterForIT,
                UniqueId = request.PublicationRequestId + "-IT",
                Created = TM.Now,
                Updated = TM.Now,
                CreatedBy = currentUser.Id,
                UpdatedBy = currentUser.Id
            };

            var newspaperLetterRequest = new Request
            {
                Title = request.Title,
                CollegeId = request.CollegeId,
                AnnouncementId = request.AnnouncementId,
                PublicationRequestId = request.PublicationRequestId,
                Status = (long)RequestStatus.DeansLetterRequestPendingSubmission,
                Type = (byte)Data.RequestType.AdvertisementLetterForNewspaper,
                UniqueId = request.PublicationRequestId + "-NS",
                Created = TM.Now,
                Updated = TM.Now,
                CreatedBy = currentUser.Id,
                UpdatedBy = currentUser.Id
            };

            var deansLetterRequest = new Request
            {
                Title = request.Title,
                AnnouncementId = request.AnnouncementId,
                PublicationRequestId = request.Id,
                Status = (long)RequestStatus.DeansLetterRequestPendingSubmission,
                Type = (byte)Data.RequestType.AdvertisementLetterForCollegeDeans,
                UniqueId = request.Id + "-DL",
                Created = TM.Now,
                Updated = TM.Now,
                CreatedBy = currentUser.Id,
                UpdatedBy = currentUser.Id
            };


            var twitterLetterRequest = new Request
            {
                Title = request.Title,
                CollegeId = request.CollegeId,
                AnnouncementId = request.AnnouncementId,
                PublicationRequestId = request.PublicationRequestId,
                Status = (long)RequestStatus.DeansLetterRequestPendingSubmission,
                Type = (byte)Data.RequestType.AdvertisementLetterForDesigner,
                UniqueId = request.PublicationRequestId + "-TW",
                Created = TM.Now,
                Updated = TM.Now,
                CreatedBy = currentUser.Id,
                UpdatedBy = currentUser.Id
            };

            var deanshipWebsiteLetterRequest = new Request
            {
                Title = request.Title,
                CollegeId = request.CollegeId,
                AnnouncementId = request.AnnouncementId,
                PublicationRequestId = request.PublicationRequestId,
                Status = (long)RequestStatus.DeansLetterRequestPendingSubmission,
                Type = (byte)Data.RequestType.AdvertisementLetterForDARWebsite,
                UniqueId = request.PublicationRequestId + "-DW",
                Created = TM.Now,
                Updated = TM.Now,
                CreatedBy = currentUser.Id,
                UpdatedBy = currentUser.Id
            };

            db.Requests.Add(newspaperLetterRequest);
            db.Requests.Add(deansLetterRequest);
            db.Requests.Add(twitterLetterRequest);
            db.Requests.Add(itLetterRequest);
            db.Requests.Add(deanshipWebsiteLetterRequest);

            db.SaveChanges();
        }
    }
}