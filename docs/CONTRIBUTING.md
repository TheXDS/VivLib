# Contributing

Thanks for considering making a contribution to VivLib.

This is a project driven mostly by curiosity, it is not intended as an all-encompassing "enterprise-grade" framework, but rather as a focused, well-documented library to help enthusiasts understand and mod classic Need For Speed game data.  
Contributions that maintain this philosophy — clarity, correctness, and minimalism — are most welcome.

I truly appreciate your time and effort in helping improve this project.  
This document outlines how you can contribute effectively and consistently.

---

## 🧭 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How to Contribute](#how-to-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Requesting Features](#requesting-features)
  - [Submitting Changes](#submitting-changes)
- [Development Setup](#development-setup)
- [Coding Guidelines](#coding-guidelines)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Release Philosophy](#release-philosophy)
- [Questions or ideas?](#questions-or-ideas)
- [License](#license)
- [IAQ](#infrequently-asked-questions)

---

<a name="code-of-conduct"></a>
## 🤝 Code of Conduct
VivLib aims to foster a friendly, open, and respectful environment for all contributors.  
Be considerate, constructive, and supportive when engaging with others.  
Harassment, discrimination, or aggressive behavior will not be tolerated.

If issues arise, please contact me directly or open a private discussion.

<a name="how-to-contribute"></a>
## 🚀 How to Contribute
There are several ways to contribute:

### Reporting Bugs
If you find a bug:
1. Check the [issue tracker](../../issues) to see if it's already reported.
2. If not, open a new issue using the **Bug Report** template. Otherwise, try to add additional information that might be missing from the original report.
3. Include as much detail as possible — steps to reproduce, expected vs actual behavior, and system information. Logs are also great if available (and, VivLib-related)

### Requesting Features
If you have an idea for an enhancement:
1. Search existing issues to see if the suggestion exists.
2. If not, open a **Feature Request** issue.
3. Describe the motivation and potential use case clearly.

### Submitting Changes
If you want to contribute code:
1. Fork the repository and create a new branch.
   ``` sh
   git checkout -b feature/99-my-feature
   ```
   If you grabbed something from the [issue tracker](https://github.com/TheXDS/VivLib/issues) (which, you should ideally do) please include the issue number in your feature branch name.
     
   If you started working on a feature that is not currently there, please take the time to create a new issue describing your proposal. That way we can track what's going on, and properly decide on a likely release cut.
2. Work on your feature. As of right now, I'm not following a strict *Merge window*. Features will be shipped on the next release cycle, whenever we decide enough new features and/or bugfixes have been added to the repo.  
   
   If your feature is large enough in scope, it's possible to create a new minor release that only includes your changes, given that there is no additional activity in the repo (little to no active forks, no new issues added)
3. Test your changes thoroughly. Ideally, have unit/integration tests made for them.
     
   Of course, some changes are non-code related, but if possible, have a second pair of eyes look at them (or, proof-read yourself too)
   > Note: Changes without unit tests will be subject to very strict scrutiny.
5. Create a [pull request](https://github.com/TheXDS/VivLib/pulls). Wait for it to be approved and merged.
6. 💵 Profit.

<a name="development-setup"></a>
## 🧩 Development Setup
VivLib does not need a complex setup. You can work on any OS supported by .NET.
You can use any editor/IDE you want, as long as it supports the version of the .NET SDK that VivLib targets.
> Note: VivLib uses the new `SLNX` format, so most legacy environments might not be able to understand it. If this is the case, do **NOT** push an `.sln` file if one is generated. You might want to add `*.sln` to your `.gitignore` if it's not already there.

1. Install an [SDK for .NET](https://dotnet.microsoft.com/) if not done so already.
   > VivLib targets `net8.0`, so I would suggest at least that version. A newer SDK is also a valid option, but it would probably require a targeting pack.
2. Create a fork, then clone the repo.
   ``` sh
   git clone https://github.com/yourusername/VivLib.git
   ```
   You may clone my repo at `https://github.com/TheXDS/VivLib.git` too, but contributions can only come from members of the repo, or forks.
3. Do a quick build (mostly as a sanity check).
   ``` sh
   dotnet build
   ```

<a name="coding-guidelines"></a>
## 🧱 Coding Guidelines
- Follow the style conventions of C#12.
- Keep functions small, clear, and testable, preferably no more than 3 levels of indentation. If you need more than that, re-think your logic. I'd like to avoid extremely long or complex methods as much as possible.
- User-facing strings should be in a resource file. I want to get rid of the user-facing strings already present in code, so I would not like to add even more. Magic strings, where a file format requires it, are acceptable.
- *Boyscoutism* is welcome, as long as it's not excessive (eg. do **NOT** rewrite the entirety of VivLib)
- Create [`XMLDocs`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/) for any public type/member you add. Note that I have somewhat of a strict line length limit of 80 characters per comment line. Try to respect that.
- Do not add comments in-code unnecessarily. Code should be simple enough where a developer can have an understanding of what's going on without a lot of comments everywhere.
- If you know something will look weird if a dev were to look at it, then you can add a comment block in-code explaining the reason why something was implemented in a certain way (see [`GeoSerializer_privates.cs`](../src/VivLib/Serializers/Geo/GeoSerializer_privates.cs), [`MapSerializer.cs`](../src/VivLib/Serializers/Audio/Mus/MapSerializer.cs) and [`BnkSerializer_Privates.cs`](../src/VivLib/Serializers/Audio/Bnk/BnkSerializer_Privates.cs) as examples)... Just don't go too crazy with it.  

  Irony and sarcasm are welcome if not excessive nor offensive. We all sometimes need a quick laugh.
- Include or update unit tests for any code changes.
- Avoid introducing new dependencies without discussion.
- When compiling VivLib, you should not introduce any new compiler warnings.
- Following SOLID is good, but when taken to the extreme it's just non-sensical.

### Notes on AI usage
I'm pro AI, as long as it's being used properly and responsibly. Heck, portions of this very document were AI-generated. Still, try yourself first. *'You'll lose what you don't use'* 🧠

If you include AI-generated code, please pay special attention to:

- Correctness of implementation
- Correctness of generated unit-test (I know you'll do this one, I would)
- Removal of redundant in-code comments — those generally just add unwanted noise.
- Most definitely, **Test the code**. AI coding agents are known to hallucinate a lot, especially with obscure or very recent frameworks.

### Notes on SOLID
When Uncle Bob came up with SOLID, he just wanted us to deal with less headaches for maintainability in the future. It's a laudable goal. But then, some people took this idea and really ran with it, to the point where it became a bit ridiculous.

Use SOLID, but responsibly. Do not create absurd levels of abstraction layers just because of "Single responsibility" and "Interface segregation". Yes, a method should only have access to the members they really need. But, if a bit of a larger interface could be used by two different methods that have only slightly different requirements then it would not make sense to segregate and create a bunch of tiny interfaces just so that you are adhering strictly to ISP or SRP.

Single responsibility is good. But if stuff is too closely related, then maybe they could be grouped together. Codecs and serializers are a good example. SRP/ISP could have dictated that I should have one interface and class each for reading and for writing. Well, as far as I know, there's only one correct way to serialize an FCE model. Alternate implementations that yield the same results and have the exact same effect would not make any sense, so having an interface just so that I can replace the implementation of the serializer part is unnecessary.

As for dependency injection... Again, if you have the single implementation of something that does what you need, there is not too much harm in not religiously using DI. It goes case-by-case, because we still want to have classes that are testeable, but sometimes a decenty well written coupled class that can be integrationally-tested is better than add a bunch of extra steps to what should be a simple functionality.

Case in point: Some religoiusly (and, allegedly "_clean_") SOLID codebases I've seen have long chains of callbacks for the sake of pure interface segregation and dependency inversion: `(Controller(SetValueTo(10)) -> Use Case(SetValueTo(parameter)) -> Gateway/Presenter(DispatchMessage(SetValueMessage(10)) -> Message(SetValueMessage dispatched with parameter 10) -> Action(When SetValueMessage Received, set Value to Received))` ... What? How about `value = 10`?

There are uses and places for this. VivLib is not necessarily one of them.

<a name="commit-guidelines"></a>
## 💬 Commit Guidelines
We want to use [Conventional Commits](https://www.conventionalcommits.org/)
 for clarity and automation. I went through enough confusion in Vivianne already 😅

Example formats:
```
feat: add new API for file decoding
fix: correct buffer overflow in decoder
docs: update contributing guidelines
test: add unit tests for EA-ADPCM encoder
```
<a name="pull-request-process"></a>
# 🔄 Pull Request Process
1. Ensure all tests pass.
2. Update documentation if behavior changes.
3. Reference any related issues using keywords (e.g. `Fixes #42`).
4. Wait for review and address any feedback.
5. Once approved, your PR will be merged.

<a name="release-philosophy"></a>
# 🕒 Release Philosophy
As I said, VivLib follows a "release when ready" approach — there’s no strict merge window.  
When enough meaningful fixes or features have been added, a new version is tagged and released.  
If your contribution is large but isolated, we might create a minor release for it; again, fork/issues activity and schedules permitting.

<a name="questions-or-ideas"></a>
# 💡 Questions or Ideas?
If you're unsure about something, open a discussion or issue before coding — I'd rather talk through ideas early than review unnecessary code later.

<a name="license"></a>
# ⚖️ License
By contributing, you agree that your contributions will be licensed under the same license as the project.

Make everyone's lawyers happy and see the [LICENSE](../LICENSE) file for details.

<a name="infrequently-asked-questions"></a>
# ❓ Infrequently Asked Questions
1. **...But, why?**  
   As I said. Mostly curiosity.
2. **How's the weather over there?**  
   Humid and hot. But, at least the temperature is somewhat stable through the year, so once you get used to it, you don't really think much about it.
3. **Are dad jokes acceptable?**  
   Yes. I do laugh at them and I'm not ashamed to admit it.
4. **Is it hard to be this beautiful?**  
   I don't know. Ask your parents.
5. **How did the Federal Republic of Central America get dissolved?**  
   The Federal Republic of Central America dissolved in the late 1830s due to civil wars between liberal reformers like General Francisco Morazán and conservative factions supported by the clergy and regional elites. Morazán's defeat and execution in 1842 marked the definitive end of the federation.
6. **Why do you dislike KIAs so much?**  
   Here's a list:
   - Because of *Piccanto* drivers
   - The KIA Boyz phenomenon
   - The Sorentos that could spontaneously combust
   - Cheap construction and abundance. You can almost find them inside a box of Corn Flakes nowadays.
7. **Is the Telluride an exception?**  
   No. Even if it's good, no.
8. **Why did he/she/it/they/thy/thou/etc. leave me?😔**  
   The heart is a tempestuous thing, friend. But, there's love in anyone that's still around you, like your family, your pets, or even that old lonely lady that wants to talk with you all day even if you have stuff to do. But, you have to love yourself before you try to get love back from others.
9. **Why is it pronounced "Wenz-day" if it's written "Wednesday"?**  
   As a famed english teacher called Bobby Finn would say: "it's because <ins>that's why</ins>. <sub>you don't see how...</sub>"
10. **I've seen some of your other projects. Can I get a copy of that cool ASCII mushroom cloud?**  
    Sure.
    ```
      _.----._
     (   (    )
    (  (    )  )
     (________)
        ||||
      --++++--
        ||||
      .(    ).
     (_(____)_)
    ```
11. **Favorite color?**  
    Thanks for asking. Ultramarine blue. I also like a highly saturated fuschia, like... violently saturated.
12. **Your most dyslexic characteristic?**  
    I frequently type an 'm' instead of a 'p', or swap 'ns' for 'sn' and 'nd' for 'dn' a lot (like, in 'isntall', or 'adn'). I also type double uppercase letters when I capitalize a word sometimes (like, 'Need For SPeed')
13. **Where can I buy toilet paper?**  
    ...the supermarket?
14. **How many Sushi rolls is it acceptable to eat in an all-you-can-eat Sushi restaurant?**  
    Yes.
15. **Most controversial, if wrong, belief?**  
    A Black Hole is probably a neutron star where it did collapse to an even denser state without being an infinitesimally small point in space. Reality not always maps 1:1 with maths (negative lengths, anyone?)
16. **I have a small lump in my eyelid, and so far no anti-biotics have worked.**  
    You'll probably need a small surgery. The worst part is the anesthesia and the fact that you'll have sharp tools very close to your eyeball. Be strong and power through it. Don't pluck it yourself, you might just re-infect it or not properly address the lachrymal obstruction.
17. **T568A or T568B for RJ-45 terminations?**  
    Well, by math T568B. I really don't care, but if I'm making a cable, I just pick the easiest one given how the manufacturer wanted to stick the pairs inside. Surprisingly, half of my cables are T568A and they work well up to 10Gbe at 10 meters on Cat5e.
18. **Are we there yet?**  
    Goddamn it... Stop asking!! Or I swear, I'll turn this car around!
19. **Recommended color of shoes to use with blue pants?**  
    I'm not a fashionist, but if you're a man, those have to be brown. I'm partial to Bullboxers. Women have more freedom, but I like white thick heels, open.
20. **Is the band Ghost just glorified Scooby-Doo chase music and Pop pretending to be Rock?**  
    Mostly, yes; I'll admit it. But, there's a few really good songs (He Is, Faith, the whole Infestisummam album) and covers (I Believe, Hanging Around)... Not limited to those mentioned in parenthesis.
21. **How much hot sauce you want with that?**  
    Yes.
22. **How do you sleep at night?**  
    Poorly. Thanks for asking.
23. **How do we know reality is "real"**?  
    "Brain in a jar" much? No? Well... The first problem is that you're asking that from within the very reality you question. Second, while not satisfactory, you might want to stop at René Descartes and be content with "I think, therefore I am". But then you might question "why do we think?" and "Are we really?". I'd think of existence as an extremely complex emergence from very simple rules at the sub-atomic level. Starting from quantum fields, all the way up to universal reality. We don't really are just us as individuals with an ethereal and individual soul. We are a lump of incommensurate fluctuations in what we understand to be multiple quantum fields that compose matter; that through so many interactions form emergent rules for how an organism should react to its environment. Does this mean that our destiny is pre-determined? Maybe yes, unless the uncertainly principle still holds up at the Armstrong level for what I would call a "Reality Voxel", otherwise we are just uncapable of ever getting perfect knowledge of every quantum field fluctuation in the entire universe at one time. If we could (and, we had enough compute power to work through the numbers) we will know the exact outcome of every possible event ever through time.
    Sorry, I drifted too far away. Just the fact that there's a "scenario" where things happen means that for us, reality is real. Just as reality would be real for a program running in a computer (their reality will cease to exist when we turn it off) but as long as we are, reality is real.
24. **Burger King, McDonald's or Wendy's?**  
    Actually, Denny's. If I have to put those three in order, then it'll be BK, then Wendy's, then eat at home, then do not eat, then McDonald's.
25. **MP3, FLAC or OGG?**  
    If it's a song I really love, then FLAC. Mp3 at 320kbps otherwise. It's not that I dislike OGG, but surprisingly, Mp3 has better support outside of Open-Source.
