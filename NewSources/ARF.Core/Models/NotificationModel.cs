using ARF.Data;

namespace ARF.Core.Models
{
    public class NotificationSummaryModel
    {
        public string LangKey { get; set; }

        public string Uri { get; set; }

        public int Count { get; set; }
    }

    public class NotificationModel
    {
        public NotificationTab Tab { get; set; }
        public string Type { get; set; }
        public long Id { get; set; }
    }
}