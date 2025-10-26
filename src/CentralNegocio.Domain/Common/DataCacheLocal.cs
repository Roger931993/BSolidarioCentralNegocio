namespace CentralNegocio.Domain.Common
{
    public class DataCacheLocal
    {
        public Guid? IdTracking { get; set; }
        public string RequestMethod { get; set; } = string.Empty;
        public string RequestUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<DataCacheLocalProcess> LogProcess { get; private set; } = new List<DataCacheLocalProcess>();

        public void AddLogProcces(DataCacheLocalProcess value)
        {
            if (LogProcess == null)
                LogProcess = new List<DataCacheLocalProcess>();

            LogProcess.Add(value);
        }


    }
}
