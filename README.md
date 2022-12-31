HumiFixPoints
=============

A lightweight C# app for calibrating transmitters like EE03, EE07 and EE08 with respect to humidity fixed points of binary saturated aqueous solutions.

## Overview

The compact humidity and temperature probes EE03, EE07 and EE08 by [E+E Elektronik](https://www.epluse.com/) are modules with a digital 2-wire bus. The probes must be connected to a "HA011001 E2 to Serial" converter. This app makes use of the Bev.Instruments.EplusE.EExx library.



## Command Line Usage

```
HumiFixPoints [options]
```

### Options

`--ports` : A comma separated list of serial port names.

`--summary (-s)` : Time interval for combining data. In hours.

`--comment` : User supplied string to be included in the log file metadata.

`--prefix` : A string prefixed to the file names.

`--MgCl2` : Humidity fix point realized by saturated MgCl$_2$ solution.

`--NaCl` : Humidity fix point realized by saturated NaCl solution.

`--KCl` : Humidity fix point realized by saturated KCl solution.

`--H2O` : Humidity fix point realized by pure water.

### Example

```
HumiFixPoints --ports="COM16, COM17" --NaCl --comment="my annotation"
```
Calibrate two transmitters connected to COM16 and COM17 using a saturated table salt solution.


## Dependencies

Bev.Instruments.Msc15: https://github.com/matusm/Bev.Instruments.Msc15

At.Matus.StatisticPod: https://github.com/matusm/At.Matus.StatisticPod

CommandLineParser: https://github.com/commandlineparser/commandline 

