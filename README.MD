# sload 

An ASP.NET Core SignalR load testing utility.

## Introduction

**sload**, or formerly known as **SignalRStresser** is a load testing utility designed to work in a distributed manner. Basically a master node invokes many tests on several agents, and these agents will perform the tests, and report back the results to the master.

If you have a server, and want to be able to test a bunch of concurrent connections to the SignalR hub hosted on it, you can use sload's load testing capabilities to see if the correct amount of virtual, or physical resources have been provisioned for it.

This project also comes with a `TestHub` project so that if desired, one can load a simple hub on the server to verify if everything is working correctly. Or, you can use it to actually apply the load test.

This has been used extensively to test not only direct connections to a native hub on a physical server, but also has been used to test the [Azure SignalR service](https://azure.microsoft.com/en-us/services/signalr-service/) to measure the relative performance between a self-hosted backplane infrastructure compared to a cloud-based one (ASRS).

## Usage

The application itself is quite straightforward. It is a command line application with most of the options defined within the source code itself. 

To see the list of options, you can invoke `.\sload -h` to get a list of options. Here is a list of common ones:

```
Usage: sload [options]

Options:
  -h|--help                       Show help information
  --isMasterNode                  Is this a master node?
  --masterNodeHostname            Hostname of the designated master node. Default: localhost
  --masterNodeListeningPort       Port of the master node. Default: 8888
  --agentNodeHostnames            The list of hostnames for the agents.
  --agentNodeListeningPort        Port of the agent to listen on. Default: 8889
  --maxConnections                The total connections to connect to the SignalR service. Default: 10
  --hubUrl                        The hub URL to connect to. Required.
  --outputDirectory               The output directory to write the results to. Default: The current working directory.
  --persistConnectionTime         Duration of time (s) to persist the connections. Default: 3
  --persistConnectionInterval     Interval duration (ms) to keep sending messages to open connections. Default: 30000
  --persistConnectionPayloadSize  Bytes for each connection to send during persistence. Default: 2048
  --workerId                      The worker ID. Default: 0
  --rampUpInterval                Interval duration to ramp up connections. Default: 1500
  --pingInterval                  Interval duration (ms) to ping the connection for activity. Default: 10000.
  --pingSize                      The number of bytes to send on Ping. Default: 1
  --keepAliveInterval             Interval duration (ms) to maintain the TCP connection. Default: 2000.
  --handshakeTimeout              Duration (ms) before a timeout for handshake. Default: 60000.
  --serverTimeout                 Duration (ms) before timing out from not hearing back from server. Default: 120000.
  --agentNodePingInterval         Duration (ms) of ping-pong operation to and from workers. Default: 30000.
```

`master` and `agent` are coupled by the fact that a master node has no reason to run if not given parameters, or an agent to pass a message to. Therefore, if you are automating tests with `sload`, you should be aware that initialiazing an agent must come before a master. Anyway, here's how to get started quickly...

### Agent Initialization

To initiate an agent, or agents:

```
.\sload --isMasterNode false --agentNodeHostnames yourAgentHostname --masterNodeHostName localhost
```

The agent will then begin to listen on port `9999` by default. The agent is designed to listen continusouly for any signals/parametesr being passed through the TCP socket from a master node. The master node host information is needed so that the agent can send back reports to the master after a test as been completed.

### Master Initialization

To initiate a `master`, run another instance of the program locally, or remotely defining the hostnames of the agent. For example, to provision 5000 connections to 2 agents:

```
.\sload --maxConnections 2500 --agentNodeHostnames agent1,agent2 --masterNodeHostName localhost --hubUrl https://localhost:5001/hub
```

2500 connections will then be generated by `agent1` and `agent2` for a total of **5000** connections to `https://localhost:5001/hub`.

## Test Results

When the tests are complete, sload generates a report similar to the following, so you can parse and analyze later:

```
{
	"runParameters": [{
		"successfulConnections": 118,
		"faultedConnections": 0,
		"disconnectedConnections": 0,
		"masterNode": false,
		"benchmarkState": 7,
		"workerId": "eb9a9f5d-3c94-41f1-b762-74db0c290bd4",
		"runParameters": {
			"isMasterNode": false,
			"masterNodeHostname": "localhost",
			"masterNodeListeningPort": 8888,
			"agentNodeHostnames": ["localhost"],
			"agentNodeListeningPort": 9999,
			"agentNodePingInterval": 30000,
			"maxConnections": 100,
			"hubUrl": "https://localhost:5001/hub",
			"outputDirectory": "C:\\sload\\SignalRStresser\\SignalRStresser\\bin\\Release\\netcoreapp2.2\\publish\\sload_results\\",
			"persistConnectionTime": 3,
			"persistConnectionInterval": 30000,
			"persistConnectionPayloadSize": 2048,
			"numberOfCpus": 8,
			"workerId": "eb9a9f5d-3c94-41f1-b762-74db0c290bd4",
			"rampUpInterval": 1500,
			"pingInterval": 10000,
			"pingSize": 1,
			"keepAliveInterval": 2000,
			"handshakeTimeout": 60000,
			"serverTimeout": 120000
		}
	}],
	"deadWorkers": [],
	"errorLogs": [
		[]
	],
	"callTimes": [{
		"workerId": "eb9a9f5d-3c94-41f1-b762-74db0c290bd4",
		"times": [16, 17, 21, 90, 56, 90, 88, 89, 88, 23, 85, 20, 17, 55, 17, 56, 89, 90, 90, 88, 63, 90, 92, 87, 18, 17, 56, 56, 90, 88, 87, 85, 16, 85, 23, 20, 17, 16, 88, 55, 90, 89, 85, 55, 14, 85, 16, 57, 26, 61, 77, 55, 56, 56, 56, 23, 56, 75, 56, 90, 56, 22, 76, 90, 57, 89, 78, 89, 61, 51, 67, 77, 21, 56, 23, 23, 23, 89, 57, 56, 89, 77, 89, 74, 27, 60, 56, 59, 75, 23, 56, 67, 77, 75, 56, 78, 56, 60, 92, 59]
	}],
	"callStatistics": {
		"successfulConnections": 118,
		"disconnectedConnections": 0,
		"faultedConnections": 0,
		"callerLatencyStats": {
			"average": 59.0,
			"p50": 59.0,
			"p90": 90.0,
			"p95": 90.0,
			"p99": 92.0,
			"totalCalls": 0.0,
			"fast": 1.0,
			"acceptable": 0.0,
			"slow": 0.0,
			"xslow": 0.0
		},
		"reconnectStatistics": {
			"average": 38.0,
			"p50": 17.0,
			"p90": 103.0,
			"p95": 105.0,
			"p99": 105.0,
			"totalReconnectsInPersistence": 18
		}
	},
	"reconnectResults": []
}
```

## Scale

* I have been able to generate 150,000 concurrent connections with caller-to-caller latency tests with sload.
* The platform that was heavily tested was using the Azure SignalR service.

## Contact

Contact me, Roger Ngo @ urbanspr1nter@gmail.com, if you want to learn more! You can also visit my webpage at: http://rogerngo.com.