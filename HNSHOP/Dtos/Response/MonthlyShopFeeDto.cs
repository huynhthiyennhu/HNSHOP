namespace HNSHOP.Dtos.Response
{
    public class MonthlyShopFeeDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Fee => Math.Round(Revenue * 0.03m, 0); 
        public string MonthLabel => $"{Month:00}/{Year}";
        public bool IsPaid { get; set; } 

    }


}
