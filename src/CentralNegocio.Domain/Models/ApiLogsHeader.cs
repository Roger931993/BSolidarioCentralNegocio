namespace CentralNegocio.Domain.Models
{
  public class ApiLogsHeader
  {
    public int HeaderId { get; set; }
    public string RequestMethod { get; set; } = string.Empty;
    public string RequestUrl { get; set; } = string.Empty;
    public int? ResponseCode { get; set; }
    public Guid IdTracking { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
