using System;
using System.Linq;
using System.Threading;
using Android.Content;
using Android.Net.Wifi;
using Android.Runtime;
using CiscoWLC.WebAuth.Client.Logging;
using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client.Core
{
    public class WifiConnector
    {
        public void Connect(Context context, Ssid ssid, OtherSettings settings)
        {
            Logger.Verbose("WifiConnector.Connecting");

            var wifiManager = context.GetSystemService(Context.WifiService).JavaCast<WifiManager>();
            EnsureWifiEnabled(wifiManager);
            if (IsConnectedToNetwork(wifiManager, ssid))
            {
                Logger.Info($"Network {ssid} already connected");
                return;
            }

            var network = GetConfiguredEnabledNetwork(wifiManager, ssid);
            EnsureDifferentNetworksNotActive(wifiManager, network);
            EnsureNetworkReachable(wifiManager, ssid);

            ActivateNetwork(wifiManager, network);
            Reconnect(wifiManager);

            Logger.Verbose($"Connection to network {ssid} requested");
            
            WaitUntilConnected(settings);
        }

        private static void EnsureWifiEnabled(WifiManager wifiManager)
        {
            if (!wifiManager.IsWifiEnabled)
                throw new InvalidOperationException("Wifi is disabled");
        }

        private static bool IsConnectedToNetwork(WifiManager wifiManager, Ssid ssid)
        {
            return wifiManager.IsWifiEnabled
                   && wifiManager.ConnectionInfo != null
                   && wifiManager.ConnectionInfo.SSID == ssid.Quoted;
        }

        private static void Reconnect(WifiManager wifiManager)
        {
            if (!wifiManager.Reconnect())
                throw new InvalidOperationException("Reconnection failed");
        }

        /// <summary> Get existing matching network </summary>
        private static WifiConfiguration GetConfiguredEnabledNetwork(WifiManager wifiManager, Ssid ssid)
        {
            var existingNetwork = wifiManager.ConfiguredNetworks
                .FirstOrDefault(x => x.Ssid == ssid.Quoted);
            Logger.Verbose(existingNetwork != null
                ? $"Found existing network {ssid}"
                : $"Existing network {ssid} not found");

            if (existingNetwork == null)
                throw new InvalidOperationException($"Network {ssid} not configured");
            if (existingNetwork.StatusField == WifiStatus.Disabled)
                throw new InvalidOperationException($"Network {ssid} is disabled");

            return existingNetwork;
        }

        private static void EnsureDifferentNetworksNotActive(WifiManager wifiManager, WifiConfiguration network)
        {
            var differentActiveNetwork = wifiManager.ConfiguredNetworks
                .Where(x => x.Ssid != network.Ssid)
                .FirstOrDefault(x => x.StatusField == WifiStatus.Current);
            if (differentActiveNetwork != null)
                throw new InvalidOperationException($"Different network {differentActiveNetwork.Ssid} is active");
        }

        private static void EnsureNetworkReachable(WifiManager wifiManager, Ssid ssid)
        {
            if (wifiManager.ScanResults.All(x => x.Ssid != ssid.Original))
                throw new InvalidOperationException($"Network {ssid} is unreachable");
        }

        private static void ActivateNetwork(WifiManager wifiManager, WifiConfiguration network)
        {
            if (!wifiManager.EnableNetwork(network.NetworkId, true))
                throw new InvalidOperationException($"Can't enable network {network.Ssid}");
        }

        private static void WaitUntilConnected(OtherSettings settings)
        {
            // TODO: Get notified when network is active instead of sleeping
            if (settings.ConnectTimeout > 0)
                Thread.Sleep(settings.ConnectTimeout);
        }
    }
}