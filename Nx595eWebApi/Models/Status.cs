using System.Collections.Generic;

namespace Nx595eWebApi.Models
{
    public class Status
    {
        public string ArmType { get; set; }
        public bool IsSystemReady { get; set; }
        public bool IsFireAlarm { get; set; }
        public bool IsIntrusionAlarm { get; set; }
        public bool IsPanicAlarm { get; set; }
        public bool IsMedicalAlarm { get; set; }
        public bool IsExitTimeDelay { get; set; }
        public bool IsExitTimeDelay2 { get; set; }
        public bool IsEntryTimeDelay { get; set; }
        public bool IsZoneBypassEnabled { get; set; }
        public bool IsZoneTrouble { get; set; }
        public bool IsZoneTamper { get; set; }
        public bool IsZoneLowBattery { get; set; }
        public bool IsZoneSupervision { get; set; }
        public bool IsChimeEnabled { get; set; }
        public string SystemStatus { get; set; }
        public Zone[] Zones { get; set; }
        public List­<Output> Outputs { get; set; }
    }
}
