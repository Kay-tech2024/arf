namespace ARF.Web.Models
{
    public class TimelineGanttViewModel
    {
        internal int startTimeInt;
        internal int endTimeInt;

        public string task { get; set; }
        public string type { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string startTimeStr { get; set; }
        public string endTimeStr { get; set; }
        public string details { get; set; }
        public int duration { get; set; }
    }
}