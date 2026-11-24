using ARF.Data;
using System;
using System.Collections.Generic;

namespace ARF.Core.Models
{
    public class RequestModel
    {
        public long? Id { get; set; }
        public long? CollegeId { get; set; }

        public string EmployeeId { get; set; }

        public string Name { get; set; }

        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }

        public RequestType? Type { get; set; }
        public List<AttachedFile> Files { get; set; }

        public bool? IsApproved { get; set; }
        public bool? IsCanceled { get; set; }

        public string SelectedAction { get; set; }
        public bool? IsReturn { get; set; }

        public DateTime? Date { get; set; }


        public string Comments { get; set; }
        public bool IsDownloaded { get; set; }
        public string ReasonCancellationReason { get; set; }
        public string CancellationReason { get; set; }

        public RequestViewStatus Status { get; set; }

    }
}