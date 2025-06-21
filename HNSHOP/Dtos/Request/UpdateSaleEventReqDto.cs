namespace HNSHOP.Dtos.Request
{
    public class UpdateSaleEventReqDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Illustration { get; set; }
        public float? Discount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int>? CustomerTypeIds { get; set; } = new List<int>();
        public List<int>? ProductIds { get; set; } = new List<int>();
    }
}
