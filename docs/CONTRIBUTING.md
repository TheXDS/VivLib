# Contributing

Thanks for considering making a contribution to VivLib.

This is a project driven mostly by curiosity, it is not intended as an all-encompassing "enterprise-grade" framework, but rather as a focused, well-documented library to help enthusiasts understand and mod classic Need for Speed game data.  
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

## 🤝 Code of Conduct
VivLib aims to foster a friendly, open, and respectful environment for all contributors.  
Be considerate, constructive, and supportive when engaging with others.  
Harassment, discrimination, or aggressive behavior will not be tolerated.

If issues arise, please contact me directly or open a private discussion.

## 🚀 How to Contribute

There are several ways to contribute:

### Reporting Bugs
If you find a bug:
1. Check the [issue tracker](../../issues) to see if it's already reported.
2. If not, open a new issue using the **Bug Report** template. Otherwise, tro to add additional information that might be missing from the original report.
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
     
   If you started working on a feature that is not currently there, please take the time to create a new issue describing your proposal, that way we can track what's going on, and properly decide on a likely release cut.
2. Work on your feature. As of right now, I'm not following a strict *Merge window*. Features will be shipped on the next release cycle, whenever we decide enough new features and/or bugfixes have been added to the repo.  
   
   If your feature is large enough in scope, it's possible to create a new minor release that only includes your changes, given that there is no additional activity in the repo (little to no active forks, no new issues added)
3. Test your changes thoroughly. Ideally, have unit/integration tests made for it.
     
   Of course, some change are non-code related, but if possible, have a second pair of eyes look at them (or, proof-read yourself too)
   > Note: Changes without unit tests will be subject to very strict scrutiny.
5. Create a [pull request](https://github.com/TheXDS/VivLib/pulls). Wait for it to be approved and merged.
6. 💵 Profit.

## Development Setup
VivLib does not need a complex setup. You can work on any OS supported by .NET.
You can use any editor/IDE you want, as long as it supports the version of the .NET SDK that VivLib targets.
> Note: VivLib uses the new `SLNX` format, so most legacy environments might not be able to understand it. If this is the case, do **NOT** push an `.sln` file if one is generated.

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

### Notes on AI usage
I'm pro AI, as long as it's being used properly. Heck, portions of this very document were AI-generated. Still, try yourself first. *'You'll lose what you don't use'* 🧠

If you include AI-generated code, please pay special attention to:

- Correctness of implementation
- Correctness of generated unit-test (I know you'll do this one, I would)
- Removal of redundant in-code comments — those generally just add unwanted noise.
- Most definitely, **Test the code**. AI coding agents are known to hallucinate a lot, especially with obscure or very recent frameworks.

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

# 🔄 Pull Request Process
1. Ensure all tests pass.
2. Update documentation if behavior changes.
3. Reference any related issues using keywords (e.g. `Fixes #42`).
4. Wait for review and address any feedback.
5. Once approved, your PR will be merged.

# 🕒 Release Philosophy
As I said, VivLib follows a "release when ready" approach — there’s no strict merge window.  
When enough meaningful fixes or features have been added, a new version is tagged and released.  
If your contribution is large but isolated, we might create a minor release for it; again, fork/issues activity and schedules permitting.

# 💡 Questions or Ideas?
If you're unsure about something, open a discussion or issue before coding — I'd rather talk through ideas early than review unnecessary code later.

# ⚖️ License
By contributing, you agree that your contributions will be licensed under the same license as the project.

See the [LICENSE](../LICENSE) file for details.

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
9. **Why is it pronounced "Wens-day" if it's written "Wednesday"?**  
   As a famed english teacher called Bobby Finn would say: "it's because that's why. ~you don't see how...~"
10. **I've seem some of your other projects. Can I get a copy of that cool ASCII mushroom cloud?**  
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
    Black holes are probably neutron stars where it did collapse to an even denser state without being an infinitesimally small point in space. Reality not always maps 1:1 with maths (negative lengths, anyone?)