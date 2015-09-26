# CiscoWLC.WebAuth.Client

A pet project that helps to connect a mobile device to a wireless network protected by Web Authentication on a Cisco Wireless LAN Controller.
Web Auth described, for example, [here](http://www.cisco.com/c/en/us/support/docs/wireless/5500-series-wireless-controllers/108501-webauth-tshoot.html).

Current implementation has a lot of limitations:
- only Android (4.0.3 or higher) is supported
- no multiple network profiles support (you can preconfigure only one network)
- need to type ssid and login page url manually
- can't work as a service, need to open an app and press a button to connect
- weak networking: timeouts instead of actual async detection
- etc
