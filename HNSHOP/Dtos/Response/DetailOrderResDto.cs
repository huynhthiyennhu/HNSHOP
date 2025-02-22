namespace HNSHOP.Dtos.Response
{
    public class DetailOrderResDto
    {
        public CompactProductResDto Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
