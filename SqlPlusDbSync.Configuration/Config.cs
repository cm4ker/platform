using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlPlusDbSync.Data;

namespace SqlPlusDbSync.Configuration
{
    public class Config
    {
        private static Config _instance;

        private Config()
        {
            SetDefaults();
        }

        public static Config Instance
        {
            get
            {
                _instance = _instance ?? new Config();
                return _instance;
            }
        }


        public string ConnectionString { get; set; }
        public string MessageServer { get; set; }
        public string MessageServerUserName { get; set; }
        public string MessageServerUserPassword { get; set; }
        public int MessageServerPort { get; set; }
        public int MaxMessageSizeInKB { get; set; }
        public int TimeoutInSeconds { get; set; }
        public int MaxUncommitedTimeInSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ScheduleMainTaskInSeconds { get; set; }


        private void SetDefaults()
        {
            MessageServer = "localhost";
            MessageServerPort = 5672;
            MaxMessageSizeInKB = 128;
            MessageServerUserName = "";
            MessageServerUserPassword = "";
            TimeoutInSeconds = 360;
            MaxUncommitedTimeInSeconds = 600;
            ScheduleMainTaskInSeconds = 20;
        }


        public void Save()
        {
            LocalFileStorage.Save(_instance);
        }

        public void Load()
        {
            _instance = LocalFileStorage.Load<Config>();
        }

    }
}
