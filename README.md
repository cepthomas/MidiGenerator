# MidiGenerator

Emulated midi controllers:
- Virtual keyboard.
- 2D touchpad.
- That's it for now.
- Can be used to drive external midi devices via USB or internal soft devices using a loopback like [LoopMidi](https://www.tobias-erichsen.de/software/loopmidi.html).
- Requires VS2022 and .NET8.


VirtualKeyboard
- Piano control based loosely on Leslie Sanford's [Midi Toolkit](https://github.com/tebjan/Sanford.Multimedia.Midi).

ClickClack
- Experimental UI component.



# External Components

- [NAudio](https://github.com/naudio/NAudio) (Microsoft Public License).
- Application icon: [Charlotte Schmidt](http://pattedemouche.free.fr/) (Copyright © 2009 of Charlotte Schmidt).
- Button icons: [Glyphicons Free](http://glyphicons.com/) (CC BY 3.0).

=================================================================

In MIDI, the instrument sound or "program" for each of the 16 possible MIDI channels is selected with the Program Change message, which has a Program Number parameter. The following table shows which instrument sound corresponds to each of the 128 possible GM Program Numbers.[3] There are 128 program numbers. The numbers can be displayed as values 1 to 128, or, alternatively, as 0 to 127. The 0 to 127 numbering is usually only used internally by the synthesizer; the vast majority of MIDI devices, digital audio workstations and professional MIDI sequencers display these Program Numbers as shown in the table (1–128). 

