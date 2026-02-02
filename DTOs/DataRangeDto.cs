namespace TodoApi.DTOs
{
    public class DateRangeDto : PagedRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}