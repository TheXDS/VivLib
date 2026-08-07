# Frequently asked questions

### 1. Why C#?
It's a type and memory-safe language, and it's the one I dominate the most. Besides, .NET has a lot of built-in stuff in the BCL, so there's not too much else to write. Not having a massive nest of dependencies is also nice *Cough!Cough!Node.JsCough!Cough!*.

### 2. What is the scope of VivLib?
Anything that could be platform and app agnostic. Primarily, I'd like to divide it into the following functional groups:
- **Models:** Anything that represents an actual, whole piece of data; like car 3d models, audio tracks, textures, and the like.
- **Codecs:** Classes that help decode and encode data into different formats, like RefPack (general compression algorithm) and EA-ADPCM (for audio).
- **Serializers:** Stuff used to read and write models.
- **Tools:** This one is a bit nuanced. There are some atomic, simple tools that can perform simple transformations on models. More on them below.

### 3. What about those tools you mentioned?
Understandably, tools sound like something that would be bespoke business logic of any consumer app. And indeed, you can consider those as BL stuff. But, those are small operations that could be useful in many contexts, not just for an editor app.

Indeed, I'm considering how to remove those and put them in a separate project inside of [Vivianne](https://github.com/TheXDS/Vivianne). Hadn't done it just yet because of some coupled dependencies in VivLib that I would have to get rid of.

### 4. Would VivLib/Vivianne support *X*, *Y* or *Z* format from NFS *< insert any version other than 2/3/4 >*? how about other EA games from the era?
The problem is documentation. I managed to get VivLib to read and (for the most part) write data for Need For Speed 3 and 4. NFS2 currently supports read of a few assets. While there have historically been tools for other games, I'm personally not quite invested into those formats/games yet. Also, as I mentioned, documentation is somewhat harder to find, and if there's some, it does require a bit of reverse engineering on my part still.

### 5. BNK has been a weak spot not only for VivLib/Vivianne, but for other tools. Is BNK support better now?
From what I've been able to verify myself, yes. VivLib, as of the writing of this FAQ, now has proper BNK support without the corruption issues that plagued it before. And also, it does not suffer from the same sample *drift* bug present in NFS wizard. Moreover, VivLib/Vivianne properly supports alternate audio streams, something that NFS Wizard treated as "unsupported compressed data", likely because the BNK spec was not fully understood at the time.

### 6. Speaking of audio... What about speech in NFS3/4? Why can't I open those BNKs in VivLib/Vivianne?
Codecs are hard. Those files in particular use ***MicroTalk***. It's a codec with sparse documentation, and while there are repos that support reading audio tracks encoded with it (like, [vgmstream](https://github.com/vgmstream/vgmstream)) I really don't have much spare time in my day to day to go through it. I know, I could try using an AI agent to go through it, but... I did try that, and the agent just gave up with the sheer amount of files that it had to sift through. So it sounds like it's going to involve some manual labour.

Still, I'm planning on somehow bringing support for it if I can.

### 7. Alright, and why are you taking so long?
Yes.

### 8. I do not believe in having my data collected, and I'm on Windows XP/Vista(!)/7. Would Vivianne work for me?
Wrong repo for asking that, but alright.

I'll be honest: Probably not, or at least not well.

There have been reports from my early beta users where the UI was just completely broken. This is because I'm using bleeding edge libraries and tools, which sadly do not properly support older versions of Windows.

~~As far as I'm aware,~~ Windows 7 does run Vivianne just fine ~~**with aero disabled**. I cannot test myself right now, as I do not have a spare machine where I could install Win7 and try, so my best test case was made by using a virtual machine which, does not have Aero.~~

> Update: Yes. Managed to verify it, and currently, it does work. The only scenario I did not test was if you customized the shell with themes that modify Windows itself. But it's more or less a given that when using something outside of the manufacturer's support, things can break.

Older versions of Windows are completely out of the question.

### 9. Have you thought of moving away from WPF? Ain't that thing dead?
Well, yes and no; to both questions.
- Indeed, WPF has been unofficially *sunset*. Microsoft would rather have us .NET devs use MAUI or WinUI3 and push our apps to the store. I'm not interested in doing that to be honest. That said, WPF is (and supossedly will remain) a supported technology for the forseable future (kinda like how older COM/MFC/ATL apps can run for the most part in Windows 11)
- I've been highly interested in moving away from WPF. Heck, I'm writing this from Fedora 44. The thing is, most multi-platform UI frameworks for C# have bugs and/or quirks, or require expensive licensing that I'm not interested in paying for Vivianne. I've been intermittently working on [Ganymede](https://github.com/TheXDS/Ganymede) precisely to have an MVVM framework that I could take to any platform I'd want, but that's still under early development. And again, why am I taking so long? Well... yes. Not much else to say.

Ok, back to VivLib...

### 10. Is this intended to compete with OpenNFS?
Not at all. OpenNFS (or, more specifically [LibOpenNFS](https://github.com/OpenNFS/LibOpenNFS)) is a project written in C++ with high performance asset loading and parsing in mind, for a game that supports assets from several NFS games in one place. Up until recently, it didn't support writing (and, there's only a scaffold on how to implement that funcitonality at the moment)

VivLib on the other hand, is thought of as a modtool-first, non-gaming library that tries to include full read and write support for those same assets. While you could technically use VivLib with something like [Unity](https://unity.com), it's not the intended use case.

### 11. And, how about those new repos popping all over github?
Again, no. This repo was made for me, and for anyone that could find it useful in any way. If any of those other repos works better for you, great! We are a collective community of enthusiasts, and at least to the best of my knowledge it's not like we are in any kind of race or competition to get the best parsers or an app that everyone likes. I use VivLib/Vivianne because I wanted to have a tool I understood fully, something that gave me the joy of learning and understanding NFS file formats while I made my own mods.

I'm happy to see that the classic NFS saga is getting attention and love. That's all that matters.
