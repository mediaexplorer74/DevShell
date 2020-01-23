# The Shell

The primary interface for this library is the `IShell` interface. It has one method, `Run` with two overloads for various customization options on starting a process. It also exposes a list of running processes. Currently DevShell uses the windows shell under the hood to start things but as time progresses I expect that will be the case less often. 