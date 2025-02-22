namespace HNSHOP.Dtos.Response
{
    public class CustomerResDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? Dob { get; set; }
        public string Description { get; set; } = string.Empty;
        public CustomerTypeResDto CustomerType { get; set; } = null!;
    }
}
