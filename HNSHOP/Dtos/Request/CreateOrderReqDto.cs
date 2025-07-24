namespace HNSHOP.Dtos.Request
{
    public class CreateOrderReqDto
    {
        public int AddressId { get; set; }
        public string? NewAddress { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public List<CreateDetailOrderReqDto> DetailOrderReqDtos { get; set; } = new();
    }

}
