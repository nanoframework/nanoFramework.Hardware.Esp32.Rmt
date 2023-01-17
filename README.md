[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_lib-Hardware.Esp32.Rmt) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Hardware.Esp32.Rmt.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Hardware.Esp32.Rmt/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

### Welcome to the .NET **nanoFramework** Hardware.Esp32.Rmt Library repository

RMT (Remote Control) is an ESP32 module driver that is, originally, intended to be used with infrared remote control signals. However, the module and APIs are generic enough that they can used to send/receive other types of signals.

## Getting Started

Our samples repository contains commented code showcasing how to use the RMT module in ESP32 MCUs to control various types of devices using nanoFramework. The RMT samples can be found [Here](https://github.com/nanoframework/Samples/tree/main/samples/Hardware.Esp32.Rmt). 

A detailed explanation about the RMT module can be found [here](https://docs.espressif.com/projects/esp-idf/en/v4.4.3/esp32/api-reference/peripherals/rmt.html).

## Migrating from v1 to v2

There are breaking changes in the managed API surface. If you have existing code that depends on v1.x of this library you will need to refactor it so it works with the new API surface in v2.x.

The changes are mostly around how receive/transmit channels are initialized. The other APIs from v1.x remain as they are.

Please update the code as follows:

**V1.x API Surface**

```csharp
// creating a transmit channel
var txChannel = new TransmitterChannel(TxPinNumber);
txChannel.ClockDivider = 80;
txChannel.CarrierEnabled = false;
txChannel.IdleLevel = false;
txChannel.AddCommand(new RmtCommand(20, true, 15, false));
// add more commands...

txChannel.Send(false);

// creating a receive channel
var rxChannel = new ReceiverChannel(RxPinNumber);
rxChannel.ClockDivider = 80; // 1us clock ( 80Mhz / 80 ) = 1Mhz
rxChannel.EnableFilter(true, 100); // filter out 100Us / noise 
rxChannel.SetIdleThresold(40000);  // 40ms based on 1us clock
rxChannel.ReceiveTimeout = new TimeSpan(0, 0, 0, 0, 60); 
rxChannel.Start(true);
```

In V2.x, the above code must be rewritten as:

```csharp
var txChannelSettings = new TransmitChannelSettings(-1, TxChannelPinNumber)
{
  ClockDivider = 80,
  EnableCarrierWave = false,
  IdleLevel = false
};

var txChannel = new TransmitterChannel(txChannelSettings);
txChannel.AddCommand(new RmtCommand(20, true, 15, false));
// add more commands...

txChannel.Send(false);


var rxChannelSettings = new ReceiverChannelSettings(pinNumber: RxChannelPinNumber)
{
  EnableFilter = true,
  FilterThreshold = 100,
  IdleThreshold = 40_000,
  ReceiveTimeout = new TimeSpan(0, 0, 0, 0, 60)
};

using var rxChannel = new ReceiverChannel(rxChannelSettings);
rxChannel.Start(clearBuffer: true);
```

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Hardware.Esp32.Rmt | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.Hardware.Esp32.Rmt/_apis/build/status/nanoFramework.Hardware.Esp32.Rmt?repoName=nanoframework%2FnanoFramework.Hardware.Esp32.Rmt&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.Hardware.Esp32.Rmt/_build/latest?definitionId=49&repoName=nanoframework%2FnanoFramework.Hardware.Esp32.Rmt&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.Hardware.Esp32.Rmt.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Hardware.Esp32.Rmt/) |

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
