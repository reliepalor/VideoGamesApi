namespace VideoGameApi.Api.Dto.Reports
{
    public class DashboardSummaryDto
    {
        public decimal TotalVideoGameRevenue { get; set; }
        public decimal TotalDigitalRevenue { get; set; }

        public decimal TotalRevenue => TotalVideoGameRevenue + TotalDigitalRevenue;

        public int TotalVideoGameOrders { get; set; }
        public int TotalDigitalOrders { get; set; }

        public int TotalUsers { get; set; }
    }
}