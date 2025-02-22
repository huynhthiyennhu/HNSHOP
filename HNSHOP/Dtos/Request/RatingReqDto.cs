namespace HNSHOP.Dtos.Request
{
    public class RatingReqDto
    {
        public int ProductId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int RatingValue { get; internal set; }
    }
}
