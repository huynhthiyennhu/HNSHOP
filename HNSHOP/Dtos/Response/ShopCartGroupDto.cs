namespace HNSHOP.Dtos.Response
{
    public class ShopCartGroupDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public List<CartItemResDto> Items { get; set; } = new();
    }

}
