[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.Hardware.Esp32.Rmt&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_lib-Hardware.Esp32.Rmt) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Hardware.Esp32.Rmt.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Hardware.Esp32.Rmt/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

### Welcome to the .NET **nanoFramework** Hardware.Esp32.Rmt Library repository

RMT (Remote Control) is an ESP32 module driver that is, originally, intended to be used with infrared remote control signals. However, the module and APIs are generic enough that they can used to send/receive other types of signals.

## Getting Started

Our samples repository contains commented code showcasing how to use the RMT module in ESP32 MCUs to control various types of devices using nanoFramework. The RMT samples can be found [Here](https://github.com/nanoframework/Samples/tree/main/samples/Hardware.Esp32.Rmt). 

A detailed explanation about the RMT module can be found [here](https://docs.espressif.com/projects/esp-idf/en/v5.4.4/esp32/api-reference/peripherals/rmt.html).

## 🌟 Overview

nanoFramework.Hardware.Esp32.Rmt provides a high‐performance managed wrapper around the ESP‐IDF RMT peripheral. Version 3.x introduces a major redesign aligned with the ESP‐IDF v5 RMT API, enabling:

- Faster symbol processing
- Native‐accelerated encoders
- Cleaner separation of symbols, channels, and encoders
- Easier LED strip control (WS2812, SK6812, etc.)
- New utilities for decoding IR protocols

This version is a breaking change from V2, but migration is straightforward.

## 📖 Samples

You can find RMT‐based samples in the nanoFramework Samples repository:

- WS2812 / SK6812 LED strip control
- Ultrasonic HC-SR04 range
- Infrared receiver decoding

👉 https://github.com/nanoframework/Samples/tree/main/samples/Hardware.Esp32.Rmt


## ESP Boards and available channels

When opening a Transmitter or Receiver channel the best channel will be automatically selected based
on the number of available Transmitter or Receiver channels. See below for a list of channels available 
for each ESP32 type. 

| Model | Total RMT channels | # Transmitter channels | # Receiver channels |
| ----- | ----- | ----- | ----- |
| ESP32 | 8 | 8 | 8 |
| ESP32_C3 | 4 | 2 | 2 |
| ESP32_C5 | 4 | 2 | 2 |
| ESP32_C6 | 4 | 2 | 2 |
| ESP32_C61 | 0 | - | - |
| ESP32_H2 | 4 | 2 | 2 |
| ESP32_S2 | 4 | 4 | 4 |
| ESP32_S3 | 8 | 4 | 4 |
| ESP32_P4 | 8 | 4 | 4 |


# 🧱 Core Concepts

## 1. RmtSymbol — the basic waveform unit

```csharp
var symbol = new RmtSymbol(
    duration0: 10, level0: true,
    duration1: 20, level1: false);
```
Each symbol represents:

A high/low level , Two durations
One complete waveform transition

## 2. RmtSymbols — a collection of symbols

```csharp
var symbols = new RmtSymbols();
symbols.Add(new RmtSymbol(10, true, 20, false));
symbols.Add(new RmtSymbol(5, false, 5, true));
```
You can index, enumerate, clear and serialize the collection.
The serialized data is cached in RmtSymbols class to improve performance. 

## 3. Creating a Transmit Channel

```csharp
var settings = new TransmitChannelSettings(pinNumber: 18)
{
    ResolutionHz = 10_000_000,
    NumberOfMemoryBlocks = 1
};
var tx = new TransmitterChannel(settings);
```
The size of the memory block depends on the target and is normally 64 or 48 symbols. If you specify
more than one then it takes memory from other channels and will reduce the number of channels that
can be opened.

The TransmitterChannel class will automatically select the next best channel available for you. If no more
channels are available then an exception will be thrown.

## 4. Creating a Receive Channel

```csharp
var settings = new ReceiverChannelSettings(pinNumber: 19)
{
    IdleThreshold = 200_000,   // 200 μs
    FilterThreshold = 1_000    // 1 μs
};

var rx = new ReceiverChannel(settings);
```

The ReceiverChannel class will automatically select the next best channel available for you. If no more
channels are available then an exception will be thrown.

## 5. Receiving RMT Symbols using blocking method

```csharp
RmtSymbols received = rx.Receive();

```

This blocks until symbols are received or the timeout is reached. The timeout is defined in the ReceiverChannelSettings.
If no symbols are received then a null is returned. 

## 6. Receiving RMT Symbols using non-blocking method

```csharp
rx.Start();

bool exit = false;

while(!exit)
{
    RmtSymbols received = rx.TryGetReceivedSymbols();
    if (received != null)
    {
        // process received symbols
    }
}

rx.Stop();
```

This will start the receiver in non-blocking mode. You can then poll for received symbols using TryGetReceivedSymbols() method.


## 7. Transmitting RMT Symbols

```csharp
var symbols = new RmtSymbols(new[]
{
    new RmtSymbol(10, true, 20, false),
    new RmtSymbol(5, false, 5, true)
});

tx.Send(symbols);
```

## 🔧 Encoders — High‐Performance Native Pipelines

Encoders allow you to build multi‐stage native pipelines by defining a number of encoders that
are run in order using passed or embedded data.

### Copy Encoder

Sends raw RMT symbols:

```csharp
var copyEnc = new CopyEncoderSettings(new[]
{
    new RmtSymbol(10, true, 20, false)
});
```

The copy encoder can have fixed symbols defined at creating or dynamic symbols loaded when sending.
Normally used to encode a pulse at start or end of data stream.

### Byte Encoder

Converts bytes → RMT symbols:

```csharp
var bit0 = new RmtSymbol(3, true, 6, false);
var bit1 = new RmtSymbol(6, true, 3, false);

var byteEnc = new ByteEncoderSettings(bit0, bit1, msbFirst: true);
```
The ByteEncoder encodes a byte array into symbols for transmission based on passed pulse widths
for 0 or 1 bits. Which order the bits are sent can also be defined.

### Using Encoders with TransmitterEncodedChannel

```csharp
var tx = new TransmitterEncodedChannel(
    settings,
    new EncoderSettings[] { byteEnc, copyEnc });

EncoderData[] data = new EncoderData[]
{
    new EncoderData(new byte[]{ 0xAA, 0x55 }),
    new EncoderData(null) // use CopyEncoderSettings embedded symbols
};
    
tx.SendWithEncoders(data);
```

The EncoderSettings array defines the Encoders to use on the pipeline.
With the SendWithEncoders you pass an EncoderData[] which defines the 
data to be supplied to each encoder defined on the Transmitter channel. 

if the EncoderData is null no data is supplied to that encoder. 
Used when Encoder already has embedded data.

The EncoderData also has a loop parameter which tells the encoder to loop that many times
sending data the data. Useful with led string to send a recurring pattern. 


# 🔗 Synchronizing Output Across Multiple RMT Channels

The ESP32 RMT peripheral allows multiple transmit channels to start output at the exact same time.  
This is essential for applications such as:

- Multi‑pin LED matrix driving  
- Parallel data transmission  
- Multi‑channel motor/servo control  
- Generating phase‑aligned signals  
- Any scenario requiring deterministic simultaneous edges

The nanoFramework V3 API exposes this capability through the  
**`TransmitSyncManager`** class.

---

## 🧩 Creating Synchronized Channels

First, create two or more `TransmitterChannel` instances:

```csharp
var ch1 = new TransmitterChannel(new TransmitChannelSettings(18));
var ch2 = new TransmitterChannel(new TransmitChannelSettings(19));
var ch3 = new TransmitterChannel(new TransmitChannelSettings(21));
```
Then pass them to the sync manager:
```csharp
var sync = new TransmitSyncManager(new[]
{
    ch1,
    ch2,
    ch3
});
```
The sync manager links the channels internally so they can start together.

### 🚀 Starting a Synchronized Transmission
Once the sync manager is created, any transmission started on any of the linked channels will 
wait until all channels are ready to send. When the last channel is ready, all channels will begin output at the same instant.
```csharp
ch1.Send(symbols1, waitTxDone: false);
ch2.Send(symbols2, waitTxDone: false);
ch3.Send(symbols3, waitTxDone: false);
```
// All channels begin output at the same moment

### 🔄 Restarting a Synchronized Transmission
Before starting another simultaneous transmission, call:
```csharp
sync.Reset();
```
This clears the internal sync latch so the next transmission can be aligned again.

### 🧹 Cleaning Up
```csharp
sync.Dispose();
```
This releases the native sync manager to unlink the channels.

## ✔ Example: Driving a 3‑channel LED installation
```csharp
var red   = new TransmitterChannel(new TransmitChannelSettings(18));
var green = new TransmitterChannel(new TransmitChannelSettings(19));
var blue  = new TransmitterChannel(new TransmitChannelSettings(21));

var sync = new TransmitSyncManager(new[] { red, green, blue });

// Prepare encoded LED data for each channel
red.SendWithEncoders(new[] { new EncoderData(redData) }, false);
green.SendWithEncoders(new[] { new EncoderData(greenData) }, false);
blue.SendWithEncoders(new[] { new EncoderData(blueData) }, false);

// All three channels begin output at the same instant
```
This ensures perfect colour alignment across multiple LED strips or panels.


## 🌈 Sending Data to Addressable LEDs (WS2812/SK6812)

Use the new high‐level helper:

```csharp
var led = new LedTransmitChannel(pinNumber: 18, LedType.WS2812);

byte[] pixelData = new byte[]
{
    255, 0, 0,   // Red
    0, 255, 0,   // Green
    0, 0, 255    // Blue
};

led.SendLedData(pixelData);
```

This class automatically:

- Configures timing based on supplied Led type
- Builds encoders on channel
- Handles reset pulses
- Sends data efficiently

For led types that use different timings that are not covered by this function then use
a constructor which allows timings to be passed.

## 🔄 Migrating from V2 to V3

V3 introduces a modernized API aligned with ESP‐IDF v5. Here are the key differences:

✔ No longer need to specify a channel. This is automatically selected internally.

✔ No longer need to specify the clock divider value. Specify resolution instead.

Specifying a resolution of 1Mhz gives a resolution of 1us across all channels.

✔ RmtCommand renamed to RmtSymbol

Renamed to match ESP‐IDF terminology which better describes its usage.

✔ Symbol collections moved from Transmitter channel

TransmitChannel no longer stores symbols directly. This allows same symbols to be sent across multiple channels and simplifies the API.

✔ New Symbols class

This includes a native symbol serializer for performance increase.  

✔ New encoder architecture

Encoders offload work to native code for major performance gains.

Available encoders:

- CopyEncoder — Sends raw RMT symbols
- ByteEncoder — Converts bytes to RMT symbols using bit patterns

✔ New TransmitterEncodedChannel

Supports multi‐stage encoding pipelines.

✔ New LedTransmitChannel

High‐level helper for WS2812/SK6812 LED strips.

✔ New RmtUtils.DecodeSymbolArrayToBytes()

Decodes received RMT symbols into bytes — ideal for IR protocol decoding.


## 🦭 Migration Summary (V2 → V3)

| V2 API | V3 API |
| ----- | ----- |
| RmtCommand | RmtSymbol |
| TransmitChannel.Send(RmtCommand[]) | TransmitterChannel.Send(RmtSymbols) |
| No encoders | CopyEncoder, ByteEncoder |
| No LED helper | LedTransmitChannel |
| No IR decoder | Utils.DecodeSymbolArrayToBytes() |
| Symbols stored inside channel | Symbols stored in RmtSymbols |
| GetAllItems() | TryGetReceivedSymbols() |

**V2.x API Surface**

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

In V3.x, the above code must be rewritten as:

```csharp
var txChannelSettings = new TransmitChannelSettings(TxChannelPinNumber)
{
    ResolutionHz = 1000000,
    EnableCarrierWave = false,
    IdleLevel = false
};

var txChannel = new TransmitterChannel(txChannelSettings);

RmtSymbols symbols = new RmtSymbols();
symbols.Add(new RmtSymbol(20, true, 15, false));
// add more commands...

txChannel.Send(symbols);


var rxChannelSettings = new ReceiverChannelSettings(RxChannelPinNumber)
{
    FilterThreshold = 100,
    IdleThreshold = 40_000,
    ReceiveTimeout = new TimeSpan(0, 0, 0, 0, 60)
};

using var rxChannel = new ReceiverChannel(rxChannelSettings);

RmtSymbols data = rxChannel.Receive();
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
