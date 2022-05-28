using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWindow.Utils
{
    public static class Constants
    {
        public static String DIRECTION = ConfigurationManager.AppSettings["DIRECTION"];
        public static int PAGE = Int32.Parse(ConfigurationManager.AppSettings["PAGE"]);
        public static int SIZE = Int32.Parse(ConfigurationManager.AppSettings["SIZE"]);
        public static bool TRIGGER_AUTO_OPEN = Boolean.Parse(ConfigurationManager.AppSettings["TRIGGER_AUTO_OPEN"]);
        public static bool VERIFY_LICENSE = Boolean.Parse(ConfigurationManager.AppSettings["VERIFY"]);
        public static bool SHOW_MESSAGE_NOT_CONNECT_COMPORT = Boolean.Parse(ConfigurationManager.AppSettings["SHOW_MESSAGE_NOT_CONNECT_COMPORT"]);
        public static bool CREATE_LOG_WHEN_NOT_CONNECT_COMPORT = Boolean.Parse(ConfigurationManager.AppSettings["CREATE_LOG_WHEN_NOT_CONNECT_COMPORT"]);
        public static int _OPEN_DURATION = Int32.Parse(ConfigurationManager.AppSettings["_OPEN_DURATION"]);
        public static bool MAIN_APP = Boolean.Parse(ConfigurationManager.AppSettings["MAIN_APP"]);

        public static Dictionary<string, string> DIRECTIONS = new Dictionary<string, string>
        {
            { "IN", "Vào" },
            { "OUT", "Ra" }
        };

        public static String LOG_FILE_NAME = "log.txt";

        public static String AUTO_OPEN_MESSAGE = "logs_open\n";

        public enum OpenType
        {
            REMOTE,
            AUTO,
            AUTO_WHITELIST,
            MANUAL
        }

        public static Dictionary<string, string> OPEN_TYPES = new Dictionary<string, string>
        {
            { "REMOTE", "Remote" },
            { "AUTO", "Tự động" },
            { "MANUAL", "Bằng app" },
            { "AUTO_WHITELIST", "Tự động" }
        };

        public static string PLACEHOLDER = "Inspection";
    }
}
