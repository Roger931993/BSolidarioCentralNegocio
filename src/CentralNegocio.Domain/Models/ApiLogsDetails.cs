namespace CentralNegocio.Domain.Models
{
  public class ApiLogsDetails
  {
    public int DetailId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int HeaderId { get; set; }
    public int StatusCode { get; set; }
    public string TypeProcess { get; set; } = string.Empty;
    public string DataMessage { get; set; } = string.Empty;
    public string ProcessComponent { get; set; } = string.Empty;
  }
}
