using ARF.Data;
using System;

namespace ARF.Core.Models
{
    public class RequestHistoryModel
    {
        public RequestHistoryTypes Type { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string UserFullname { get; set; }
        public string Url { get; set; }
    }

 
}