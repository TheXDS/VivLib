---
name: commit-guides
description: 'Generate and validate Conventional Commits messages for VivLib. Use when: creating commit messages, reviewing commit history, validating commit format, suggesting conventional commit prefixes.'
argument-hint: 'Describe what you changed for a commit message'
---

# VivLib Conventional Commits Guide

## When to Use
- Generating commit messages after code changes
- Validating existing commit messages follow Conventional Commits
- Determining the correct prefix for a set of changes

## Procedure

### Step 1: Classify the Change
Map the changes to the appropriate prefix:

| Prefix   | Use When | Example |
|----------|----------|---------|
| `feat:`  | New public API, new file format support, new tool | `feat: add BNK WAV export` |
| `fix:`   | Bug fix, correction, regression | `fix: RefPack decompress offset error` |
| `docs:`  | Documentation only (README, CONTRIBUTING, AGENTS) | `docs: update contributing guidelines` |
| `test:`  | Adding or fixing tests (no production code) | `test: add RefPack roundtrip test` |
| `chore:` | Build, deps, CI, config, no code change visible to users | `chore: update SixLabors.ImageSharp to 3.1.1` |
| `refactor:` | Code change that neither fixes a bug nor adds a feature | `refactor: extract common header parsing` |
| `style:` | Formatting, whitespace, semicolons, no logic change | `style: fix indentation in FceSerializer` |
| `perf:`  | Performance improvement | `perf: optimize texture decompression loop` |
| `ci:`    | GitHub Actions, CI config | `ci: add codecov upload step` |
| `build:` | Build system, package management | `build: update target framework to net8.0` |

### Step 2: Write the Message
Follow this structure:

```
<type>: <short description>

<optional longer description>
```

Rules:
- **Short description:** imperative mood, no capital first letter, no period
- **Optional body:** explain *why* and *what*, not *how*
- **Reference issues:** `Fixes #42` or `Refs #17`

### Step 3: Validate
Check:
1. Prefix is one of the approved types above
2. Description starts with lowercase
3. Description is under 72 characters
4. No trailing period in the subject line
5. Body (if any) is blank-separated from subject

### Quick Reference
```
feat: add BNK WAV export
fix: correct RefPack decompression offset
docs: update contributing guidelines
test: add RefPack roundtrip test
refactor: extract common header parsing
chore: update SixLabors.ImageSharp to 3.1.1
```

## VivLib-Specific Notes
- New file format serializers → `feat:`
- Serializer bug fixes → `fix:`
- Unit test additions → `test:` (changes without tests face strict scrutiny)
- Build prop/target updates → `build:`
- MSBuild configuration → `build:` or `chore:`
