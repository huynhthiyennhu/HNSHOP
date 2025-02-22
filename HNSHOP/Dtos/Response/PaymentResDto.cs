namespace HNSHOP.Dtos.Response
{
    public class PaymentResDto
    {
        public string Message { get; set; } = string.Empty; // Thông báo kết quả thanh toán
        public string ResponseCode { get; set; } = string.Empty; // Mã phản hồi từ VNPAY
    }
}
