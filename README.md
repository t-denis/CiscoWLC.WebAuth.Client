# CiscoWLC.WebAuth.Client

A pet project (proof-of-concept) that helps to connect a mobile device to a wireless network protected by Web Authentication on a Cisco Wireless LAN Controller.
Web Auth described [here](http://www.cisco.com/c/en/us/support/docs/wireless/5500-series-wireless-controllers/108501-webauth-tshoot.html).

Notes:
- only Android (4.0.3 or higher) is supported for now
- no multiple network profiles support (you can preconfigure only one network)
- need to type SSID and login page URL manually
- can't work as a service, need to open an app and press a button to connect
- weak network code: timeouts instead of actual detection
- network with the specified SSID should be preconfigured (need to connect manually for the first time)
