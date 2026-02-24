namespace VideoGameApi.Api.Dto.Reports
{
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public decimal VideoGameRevenue { get; set; }
        public decimal DigitalRevenue { get; set; }

        public decimal TotalRevenue => VideoGameRevenue + DigitalRevenue;
    }
}