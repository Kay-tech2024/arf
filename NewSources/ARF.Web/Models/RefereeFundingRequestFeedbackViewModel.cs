using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class RefereeFundingRequestFeedbackPartialViewModel
    {
        public bool IsReadonly { get; set; }
        public int CutOffMarks { get; set; }
        public List<RefereeFundingRequestFeedbackViewModel> FeedbackFormQuestions { get; set; }
    }

    public class RefereeFundingRequestFeedbackViewModel
    {
        public int FeedbackFormQuestionId { get; set; }
        public int RefereeId { get; set; }
        public long RequestId { get; set; }
        public string QuestionText { get; set; }
        public int MaxMarks { get; set; }
        public int OrderNumber { get; set; }
        public string Comments { get; set; }
        public int GivenMarks { get; set; }
    }
}