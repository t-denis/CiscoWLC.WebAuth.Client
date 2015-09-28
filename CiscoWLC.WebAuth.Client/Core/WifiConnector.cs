using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Runtime;
using CiscoWLC.WebAuth.Client.Logging;

namespace CiscoWLC.WebAuth.Client.Core
{
    public class WifiConnector
    {
        public async Task<ConnectionResult> ConnectAsync(Context context, Ssid ssid, TimeSpan checkInterval, TimeSpan timeout)
        {
            Logger.Verbose("WifiConnector.Connecting");

            var wifiManager = context.GetSystemService(Context.WifiService).JavaCast<WifiManager>();
            EnsureWifiEnabled(wifiManager);
            if (IsConnectedToNetwork(wifiManager, ssid))
            {
                Logger.Verbose($"Network {ssid} already connected");
                return ConnectionResult.AlreadyConnected;
            }

            var network = GetConfiguredNetwork(wifiManager, ssid);
            EnsureDifferentNetworksNotActive(wifiManager, network);
            EnsureNetworkReachable(wifiManager, ssid);

            ActivateNetwork(wifiManager, network);
            Reconnect(wifiManager);

            Logger.Verbose($"Connection to network {ssid} requested");
            
            var result = await WaitUntilConnectedAsync(wifiManager, network, checkInterval, timeout);
            Logger.Verbose(result == ConnectionResult.Connected
                ? $"Connected to {ssid.Quoted}"
                : $"Not yet connected to {ssid.Quoted}. Try increase a connection timeout");
            return result;
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
        private static WifiConfiguration GetConfiguredNetwork(WifiManager wifiManager, Ssid ssid)
        {
            var existingNetwork = wifiManager.ConfiguredNetworks
                .FirstOrDefault(x => x.Ssid == ssid.Quoted);
            Logger.Verbose(existingNetwork != null
                ? $"Found existing network {ssid}"
                : $"Existing network {ssid} not found");

            if (existingNetwork == null)
                throw new InvalidOperationException($"Network {ssid} not configured");
            
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

        private static async Task<ConnectionResult> WaitUntilConnectedAsync(WifiManager wifiManager, WifiConfiguration network, TimeSpan checkInterval, TimeSpan timeout)
        {
            // TODO: Get notified when network is active instead of looping and sleeping

            if (timeout != TimeSpan.Zero)
            {
                if (checkInterval == TimeSpan.Zero)
                {
                    if (timeout.TotalMilliseconds > 0)
                        await Task.Delay(timeout);
                }
                else
                {
                    var startTime = DateTime.Now;
                    while (DateTime.Now - startTime < timeout)
                    {
                        if (IsCompletelyConnected(wifiManager, network))
                            return ConnectionResult.Connected;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                    }
                }
            }
            if (IsCompletelyConnected(wifiManager, network))
                return ConnectionResult.Connected;
            return ConnectionResult.NotYetConnected;
        }

        private static bool IsCompletelyConnected(WifiManager wifiManager, WifiConfiguration network)
        {
            return wifiManager.WifiState == WifiState.Enabled
                   && wifiManager.ConnectionInfo.SSID == network.Ssid
                   && wifiManager.ConnectionInfo.SupplicantState == SupplicantState.Completed
                   && wifiManager.DhcpInfo.IpAddress > 0;
        }
    }

    public enum ConnectionResult
    {
        AlreadyConnected,
        Connected,
        NotYetConnected
    }
}