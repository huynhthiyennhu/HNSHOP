namespace HNSHOP.Dtos.Request
{
    public class CreateSaleEventReqDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Illustration { get; set; } = string.Empty;
        public float Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> CustomerTypeIds { get; set; } = [];
        public List<int> ProductIds { get; set; } = [];
    }
}
