using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARF.Core.Workflows
{
    public class LetterRequestWorkflow : IRequestWorkflow
    {
        public List<Data.RequestAction> GetAllowedActions(Request request)
        {
            var dict = GetAllowedActionsTransitions(request);
            return dict.Select(a => a.Item1).ToList();
        }

        public List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request)
        {
            if (request == null && (request.Type != (byte)Data.RequestType.AdvertisementLetterForIT && request.Type != (byte)Data.RequestType.AdvertisementLetterForNewspaper && request.Type != (byte)Data.RequestType.AdvertisementLetterForCollegeDeans && request.Type != (byte)Data.RequestType.AdvertisementLetterForDesigner && request.Type != (byte)Data.RequestType.AdvertisementLetterForDARWebsite))
                return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();


            var list = new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();

            foreach (var st in GetRequestStatus(request))
            {
                switch (st)
                {
                    case RequestStatus.DeansLetterRequestPendingSubmission:
                        if (Utils.HasRolesAny(ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.SendTo_ViceDean_for_research_centers_and_databases,
                                RequestStatus.DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases,
                                null
                            ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases:
                        if (Utils.HasRolesAny(ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ApproveAndSendToDeanshOfScientificResearch,
                                RequestStatus.DeansLetterRecievedBy_DeanOfScientificResearch,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                               Data.RequestAction.ReturnToDirectorOfFinancingProgram,
                               RequestStatus.DeansLetterRequestPendingSubmission,
                               null
                           ));
                        }
                        else
                        {
                            return new List<Tuple<Data.RequestAction, RequestStatus, string[]>>();
                        }

                        break;

                    case RequestStatus.DeansLetterRecievedBy_DeanOfScientificResearch:
                        if (Utils.HasRolesAny(ArfUserRole.DeanofDeanshipofScientificResearch))
                        {
                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.Approve,
                                RequestStatus.Completed,
                                null
                            ));

                            list.Add(new Tuple<Data.RequestAction, RequestStatus, string[]>(
                                Data.RequestAction.ReturnToViceDeanForResearchCentersAndDatabases,
                                RequestStatus.DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases,
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
            var files = Utils.RequestUtils.GetFiles(request, true);


            if (request.Type == (byte)Data.RequestType.AdvertisementLetterForDARWebsite)
            {

                Utils.RequestUtils.SendNotification(
                    request,
                    TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_f_Notification,
                    new ArfUserRole[] { ArfUserRole.DarWebsiteManager },
                    null,
                    null,
                    null,
                    files.Select(f=>f.FilePath).ToArray()
                );
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForNewspaper)
            {

                Utils.RequestUtils.SendNotification(
                    request,
                    TemplateKey.UniversityMirrorNewspaper,
                    new ArfUserRole[] { ArfUserRole.DarWebsiteManager },
                    null,
                    null,
                    null,
                    files.Select(f => f.FilePath).ToArray()
                );
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForIT)
            {

                Utils.RequestUtils.SendNotification(
                    request,
                    TemplateKey.UniversityMirrorNewspaper,
                    new ArfUserRole[] { ArfUserRole.DarWebsiteManager },
                    null,
                    null,
                    null,
                    files.Select(f => f.FilePath).ToArray()
                );
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForDesigner)
            {

                Utils.RequestUtils.SendNotification(
                    request,
                    TemplateKey.UniversityMirrorNewspaper,
                    new ArfUserRole[] { ArfUserRole.Designer },
                    null,
                    null,
                    null,
                    files.Select(f => f.FilePath).ToArray()
                );
            }
            else if (request.Type == (byte)Data.RequestType.AdvertisementLetterForCollegeDeans)
            {

                Utils.RequestUtils.SendNotification(
                    request,
                    TemplateKey.UniversityMirrorNewspaper,
                    new ArfUserRole[] { ArfUserRole.CollegeDean, },
                    new ArfUserRole[] { ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch },
                    null,
                    null,
                    files.Select(f => f.FilePath).ToArray()
                );
            }


        }
    }

}
