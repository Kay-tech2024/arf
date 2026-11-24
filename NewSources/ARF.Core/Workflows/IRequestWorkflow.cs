using ARF.Core.Models;
using ARF.Data;
using ARF.Data.DbModels;
using System;
using System.Collections.Generic;

namespace ARF.Core.Workflows
{
    public interface IRequestWorkflow
    {
        List<Data.RequestAction> GetAllowedActions(Request request);

        RequestStatus[] GetRequestStatus(Request request);

        List<Tuple<Data.RequestAction, RequestStatus, string[]>> GetAllowedActionsTransitions(Request request);
        
        void OnCompleted(Request request);
    }
}
