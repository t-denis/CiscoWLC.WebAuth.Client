using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client.Core
{
    public class NetworkInfo
    {
        public NetworkInfo()
        {
        }

        public NetworkInfo(ConnectionSettings connectionSettings)
        {
            NetworkType = NetworkType.Open;
            Ssid = connectionSettings.Ssid;
        }

        public NetworkType NetworkType { get; private set; }
        public string Ssid { get; private set; }
        /// <summary> For WEP, WPA networks </summary>
        public string PreSharedKey { get; private set; }
        public string QuotedSsid => Ssid == null ? null : "\"" + Ssid + "\"";
        public string QuotedPreSharedKey => PreSharedKey == null ? null : "\"" + PreSharedKey + "\"";
    }
}