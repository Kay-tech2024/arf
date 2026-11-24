using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class RequestsByTypeViewModel
    {
        public System.Int64 Id { get; set; }
        public System.Byte Type { get; set; }
        public System.String Title { get; set; }
        public System.String FirstName { get; set; }
        public System.String SecondName { get; set; }
        public System.String ThirdName { get; set; }
        public System.String LastName { get; set; }
        public System.Int64 Status { get; set; }
        public System.Byte? FieldType { get; set; }
        public System.DateTime? ProjectStartDate { get; set; }
        public System.DateTime? ProjectEndDate { get; set; }
        public System.Double? Amount { get; set; }
        public System.DateTime Created { get; set; }
        public System.String CreatedBy { get; set; }
        public System.DateTime Updated { get; set; }
        public System.String UpdatedBy { get; set; }
        public System.Int32? IsMidTermFundApproved { get; set; }
        public System.Int32? IsFullTermFundApproved { get; set; }
        public System.Int32? IsMidTermFundPaid { get; set; }
        public System.Int32? IsFullTermFundPaid { get; set; }
    }
}