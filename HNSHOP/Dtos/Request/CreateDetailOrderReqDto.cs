namespace HNSHOP.Dtos.Request
{
    public class CreateDetailOrderReqDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
