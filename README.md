# MidiGenerator

Emulated midi controllers:
- Virtual keyboard.
- 2D touchpad.
- That's it for now.
- Can be used to drive external midi devices via USB or internal soft devices using a loopback like [LoopMidi](https://www.tobias-erichsen.de/software/loopmidi.html).
- Requires VS2022 and .NET8.

# Components

VirtualKeyboard
- Piano control based loosely on Leslie Sanford's [Midi Toolkit](https://github.com/tebjan/Sanford.Multimedia.Midi).
- Computer keyboard also supported.

ClickClack
- Experimental UI component.

# Terminology

- Channel numbers are 1-16.
- Note and controller numbers are standard midi 0-127.
- Volume is a float from 0.0 to 1.0 (or 2.0). It is converted to standard midi velocity 0-127.


# External Components

- [NAudio](https://github.com/naudio/NAudio) (Microsoft Public License).
- Application icon: [Charlotte Schmidt](http://pattedemouche.free.fr/) (Copyright © 2009 of Charlotte Schmidt).
- Button icons: [Glyphicons Free](http://glyphicons.com/) (CC BY 3.0).
