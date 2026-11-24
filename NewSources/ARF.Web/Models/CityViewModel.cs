namespace ARF.Web.Models
{
    public class CityViewModel
    {
        public long? Id { get; set; }
        public long? CountryId { get; set; }

        public string Name { get; set; }

        public long? RegionId { get; set; }

        public string RegionName { get; set; }
    }
}