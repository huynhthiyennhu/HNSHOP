using HNSHOP.Utils.EnumTypes;

namespace HNSHOP.Dtos.Response
{
    public class SubOrderResDto
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public SubOrderStatus Status { get; set; }
        public decimal Total { get; set; }

        public List<DetailOrderResDto> DetailOrders { get; set; } = [];
    }
}
