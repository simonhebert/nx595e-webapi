using System.Collections.Generic;

namespace Nx595eWebApi.Models
{
    public class Status
    {
        public string ArmType { get; set; }
        public bool IsSystemReady { get; set; }
        public bool IsFireAlarm { get; set; }
        public bool IsIntrusionAlarm { get; set; }
        public bool IsExitTimeDelay { get; set; }
        public bool IsEntryTimeDelay { get; set; }
        public bool IsZoneBypassEnabled { get; set; }
        public bool IsChimeEnabled { get; set; }
        public string SystemStatus { get; set; }
        public Zone[] Zones { get; set; }
        public List­<Output> Outputs { get; set; }
    }
}
