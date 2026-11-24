using ARF.Core.Models;

namespace ARF.Core.Models
{
    public class RewardRequestModel : RequestModel
    {
        public long PublicationRequestId { get; set; }
        public long? ResearcherId { get; set; }
        public long? RefereeId { get; set; }
    }
}
