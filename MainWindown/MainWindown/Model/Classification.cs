using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MainWindow.Model
{
    public class Classification
    {
        [JsonProperty("results")]
        public List<DefectImages> Results { set; get; }

        public class DefectImages
        {
            [JsonProperty("image")]
            public String Image;

            [JsonProperty("license_plate_image")]
            public String LicensePlateImage;
        }
    }
}
