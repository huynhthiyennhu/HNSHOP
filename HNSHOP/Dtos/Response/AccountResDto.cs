namespace HNSHOP.Dtos.Response
{
    public class AccountResDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ShopResDto? Shop { get; set; }
        public CustomerResDto? Customer { get; set; }

    }
}
