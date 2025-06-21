namespace HNSHOP.Utils
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; } = "";
        public int Port { get; set; } = 587;
        public string SenderEmail { get; set; } = "";
        public string SenderName { get; set; } = "HNSHOP";
        public string Password { get; set; } = "";
    }
}
