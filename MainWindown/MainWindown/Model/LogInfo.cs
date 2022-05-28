using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MainWindow.Model
{
    public class LogInfo
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("station_id")]
        public int StationId { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("open_type")]
        public string OpenType { get; set; }

        [JsonProperty("gate_id")]
        public int GateId { get; set; }

        [JsonProperty("defect")]
        public string LicensePlate { get; set; }

        [JsonProperty("log_time")]
        public string LogTime { get; set; }

        [JsonProperty("overview_images")]
        public List<string> OverviewImages { get; set; }

        [JsonProperty("inspect_images")]
        public List<string> InspectImages { get; set; }

        public bool IsGsImage { get; set; }

        public List<byte[]> OverviewByteImages { get; set; }

        public List<byte[]> InspectByteImages { get; set; }
    }
}
