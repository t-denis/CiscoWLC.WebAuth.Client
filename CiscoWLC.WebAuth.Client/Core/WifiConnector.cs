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
        public void Connect(Context context, NetworkInfo network, OtherSettings settings)
        {
            Logger.Verbose("WifiConnector.Connecting");

            var wifiManager = context.GetSystemService(Context.WifiService).JavaCast<WifiManager>();
            if (!wifiManager.IsWifiEnabled)
            {
                var wifiEnabled = wifiManager.SetWifiEnabled(true);
                if (!wifiEnabled)
                    throw new InvalidOperationException("Can't enable wifi");
                
                // TODO: Get notified when scan completed instead of sleeping
                if (settings.ScanTimeout > 0)
                    Thread.Sleep(settings.ScanTimeout);
            }
            if (IsConnectedToNetwork(wifiManager, network))
            {
                Logger.Info($"Already connected to network {network.QuotedSsid}");
                return;
            }
            
            wifiManager.Disconnect();

            // TODO: Remove only conflicting networks (same SSID, different settings)
            if (settings.RecreateNetworks)
                RemoveNetworks(wifiManager, network.QuotedSsid);
            var wifiConfiguration = GetExistingNetwork(wifiManager, network);
            if (wifiConfiguration == null)
            {
                // TODO: Check if network is available, otherwise don't create a configuration (maybe except networks with a hidden ssid)

                wifiConfiguration = CreateWifiConfiguration(network);
                wifiConfiguration = AddWifiConfiguration(wifiManager, wifiConfiguration);
            }
            var enabled = wifiManager.EnableNetwork(wifiConfiguration.NetworkId, true);
            if (!enabled)
                throw new InvalidOperationException($"Can't enable network {wifiConfiguration.Ssid}");
            var reconnected = wifiManager.Reconnect();
            if (!reconnected)
                throw new InvalidOperationException($"Can't reconnect to the network {wifiConfiguration.Ssid}");

            Logger.Verbose($"Connection to network {network.QuotedSsid} requested");
            // TODO: Get notified when network is active instead of sleeping
            if (settings.ConnectTimeout > 0)
                Thread.Sleep(settings.ConnectTimeout);
        }

        private bool IsConnectedToNetwork(WifiManager wifiManager, NetworkInfo network)
        {
            return wifiManager.IsWifiEnabled
                   && wifiManager.ConnectionInfo != null
                   && wifiManager.ConnectionInfo.SSID == network.QuotedSsid;
        }

        private void RemoveNetworks(WifiManager wifiManager, string ssid)
        {
            var existingNetworks = wifiManager.ConfiguredNetworks
                .Where(x => x.Ssid == ssid)
                .ToList();
            foreach (var config in existingNetworks)
            {
                Logger.Verbose($"Removing network {ssid} with id {config.NetworkId}");
                wifiManager.RemoveNetwork(config.NetworkId);
            }
        }

        /// <summary> Get existing matching network </summary>
        private WifiConfiguration GetExistingNetwork(WifiManager wifiManager, NetworkInfo network)
        {
            // PreSharedKey is hidden for ConfiguredNetworks
            // TODO: Check network type (Open/WPA/etc)
            var existingNetworks = wifiManager.ConfiguredNetworks
                .Where(x => x.Ssid == network.QuotedSsid);
            var existingNetwork = existingNetworks.FirstOrDefault();
            if (existingNetwork != null)
                Logger.Verbose($"Found existing network {network.QuotedSsid}");
            else
                Logger.Verbose($"Existing network {network.QuotedSsid} not found");
            return existingNetwork;
        }

        private WifiConfiguration CreateWifiConfiguration(NetworkInfo network)
        {
            var configuration = new WifiConfiguration
            {
                Ssid = network.QuotedSsid
            };
            switch (network.NetworkType)
            {
                case NetworkType.Open:
                    configuration.AllowedKeyManagement.Set((int)KeyManagementType.None);
                    break;
                case NetworkType.WEP:
                    configuration.WepKeys[0] = network.QuotedPreSharedKey;
                    configuration.WepTxKeyIndex = 0;
                    configuration.AllowedKeyManagement.Set((int)KeyManagementType.None);
                    configuration.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                    break;
                case NetworkType.WPA:
                    configuration.PreSharedKey = network.QuotedPreSharedKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Logger.Verbose($"Created {network.NetworkType} network {network.QuotedSsid}");
            return configuration;
        }

        /// <summary> Add wifi configuration to the device </summary>
        /// <param name="wifiManager"></param>
        /// <param name="wifiConfiguration">Configuration to add</param>
        /// <returns>Another instance of WifiConfiguration with different NetworkId</returns>
        private WifiConfiguration AddWifiConfiguration(WifiManager wifiManager, WifiConfiguration wifiConfiguration)
        {
            var networkId = wifiManager.AddNetwork(wifiConfiguration);
            wifiConfiguration = wifiManager.ConfiguredNetworks.Single(x => x.NetworkId == networkId);
            Logger.Verbose($"Network {wifiConfiguration.Ssid} added");
            return wifiConfiguration;
        }
    }
}