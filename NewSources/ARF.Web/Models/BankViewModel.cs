using ARF.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class BankViewModel
    {
        public string AccountType { get; set; }
        public int BankId { get; set; }
        public string AccountOwnerName { get; set; }
        public string IbanNo { get; set; }
        public int? CountryId { get; set; }
        public long? RequestId { get; set; }
        public int? CityId { get; set; }
        public int? NoOfParticipant { get; set; } 
        public bool IsScientificProduction { get; set; }
        public bool HasParticipant { get; set; }
        public AttachedFile FileUpload { get; set; }
        
    }
}