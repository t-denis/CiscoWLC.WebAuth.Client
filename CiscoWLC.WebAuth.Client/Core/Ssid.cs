namespace CiscoWLC.WebAuth.Client.Core
{
    public class Ssid
    {
        public Ssid(string ssid)
        {
            Original = ssid;
            Quoted = "\"" + ssid + "\"";
        }
        public string Original { get; private set; }
        public string Quoted { get; private set; }

        public override string ToString()
        {
            return Quoted;
        }
    }
}