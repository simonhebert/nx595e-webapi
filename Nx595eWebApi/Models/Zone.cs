namespace Nx595eWebApi.Models
{
    public class Zone
    {
        public int Index { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // Ready/Not Ready/Tamper/Trouble/Bypass/Inhibited/Alarm/Low Battery/Supervision Fault
        public bool IsBypassed { get; set; }
    }
}
