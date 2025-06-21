namespace HNSHOP.Dtos.Request
{
    public class CreateOrderReqDto
    {
        public int AddressId { get; set; }
        public List<CreateDetailOrderReqDto> DetailOrderReqDtos { get; set; } = new();
        public string? NewAddress { get; set; }
    }
}
