# NetDetector
NetDetector is a daemon which checks for and responds to a given MAC or IP address on the network.

### Help text
```bash
> NetDetector  -h
Usage: NetDetector [-h] [-v] [-c CONNECT] [-d DISCONNECT] [-r RATE]
                   [-C CONNECT-COUNT] [-D DISCONNECT-COUNT]
                   [-i INTERFACE] ADDR
Positional arguments:
  ADDR
        The IP or MAC address to detect.
Optional arguments:
  -h, --help
        Displays this help message and exits.
  -v, --version
        Displays the version.
  -c CONNECT, --connect CONNECT
        The command to run when the MAC address is detected on the network.
  -d DISCONNECT, --disconnect DISCONNECT
        The command to run when the MAC address is no longer detected on the network.
  -r RATE, --rate RATE
        The poll rate in seconds.
        Default: 60
  -C CONNECT-COUNT, --connect-count CONNECT-COUNT
        The number of consecutive connection detections that must elapse before the
          connect command is run.
        Default: 1
  -D DISCONNECT-COUNT, --disconnect-count DISCONNECT-COUNT
        The number of consecutive disconnection detections that must elapse before
          the disconnect command is run.
        Default: 1
  -i INTERFACE, --interface INTERFACE
        The network interface to use as the scope identifier.
        Default: 1
```
### Detection
`test.sh`
```bash
#!/bin/bash

cmd="$1"

if [ "$cmd" = "connect" ]
then
        echo "$NET_DETECTOR_ADDRESS detected"
elif [ "$cmd" = "disconnect" ]
then
        echo "$NET_DETECTOR_ADDRESS not detected"
fi
```
`NetDetector`
```bash
> NetDetector fe80::9c11:e50:ad4f:f749 -i eth2 -r 1 -c "./test.sh connect" -d "./test.sh disconnect"
(fe80::9c11:e50:ad4f:f749%3) - Watching
(fe80::9c11:e50:ad4f:f749%3) - Not detected
(fe80::9c11:e50:ad4f:f749%3) - Running: ./test.sh disconnect
fe80::9c11:e50:ad4f:f749%3 not detected
(fe80::9c11:e50:ad4f:f749%3) - Detected
(fe80::9c11:e50:ad4f:f749%3) - Running: ./test.sh connect
fe80::9c11:e50:ad4f:f749%3 detected
```