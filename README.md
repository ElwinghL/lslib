LSLib
=====

About this fork
----------------

This is [ElwinghL](https://github.com/ElwinghL)'s fork of [Norbyte/lslib](https://github.com/Norbyte/lslib), maintained as part of [BG3Tools](https://github.com/ElwinghL) — a BG3 mod-management TUI that compares three approaches to reading `.pak` (LSPK) archives: this library's `Divine.exe` (the reference implementation), a pure-Python reader ([bg3pythonpaklib](https://github.com/ElwinghL/bg3pythonpaklib)), and a native Rust reader ([bg3rustpaklib](https://github.com/ElwinghL/bg3rustpaklib)).

**Our goal here is narrow and explicit: read, write, and edit `.pak` files — nothing else in LSLib is a priority for us.** LSLib is a much bigger project (savegames, GR2 meshes/animations, Osiris story databases, virtual textures), and we have no intention of maintaining parity with upstream on any of that. If a change we need breaks or drags down one of those other areas, that's an acceptable trade-off for this fork — we'd rather have a lean, buildable, `.pak`-focused tool than a complete-but-unbuildable one.

Concretely, changes here so far:

- **Fixed `extract-packages` (batch `.pak` extraction) crashing on Linux/Wine**: `CommandLineActions.SetUpAndValidate` unconditionally parsed `--input-format` through `GetResourceFormatByString`, which only recognizes resource-conversion formats (`LSX`/`LSB`/`LSF`/`LSJ`) — never `pak` — throwing an unhandled `ArgumentException` (a full CLR crash under Wine) before `CommandLinePackageProcessor.BatchExtract` (which only needs the raw `--input-format` string for its glob, not the enum) ever ran. Now skipped for `extract-packages`, matching how `--output-format` was already skipped for that action.

Known follow-up work (see BG3Tools' `.claude/TODO.md` for the up-to-date version of this):

- **Building on Linux is currently blocked** for the *full* solution: `LSLibNative` (native C++ `.vcxproj`) needs MSVC, and the Osiris story/goal parser needs the Windows-only GPLex/GPPG tools (see Requirements below) — neither is available outside Windows. Options being weighed: running GPLex/GPPG under Wine (they're plain console tools, likely to just work), a Windows VM/CI runner for building releases, or deliberately trimming the Story/Granny/VirtualTextures code out of this fork entirely since we don't need it (the "so be it" option, in line with the narrow goal above).
- No CI/release pipeline of our own yet — for now we track upstream's official pre-built `Divine.exe` releases in [Tools/TOOLS.md](https://github.com/ElwinghL/BG3Tools/blob/main/Tools/TOOLS.md) and only build from this fork's source locally when testing a specific fix like the one above.

The rest of this README is upstream's original documentation and still applies to whatever hasn't diverged.

---

This package provides utilities for manipulating Divinity Original Sin 1, Enhanced Edition, Original Sin 2 and Baldur's Gate 3 EA files:

 - Extracting/creating PAK packages
 - Extracting/creating LSV savegame packages
 - Converting LSB, LSF, LSX, LSJ resource files
 - Importing and exporting meshes and animations (conversion from/to GR2 format)
 - Editing story (OSI) databases

Requirements
============

To build the tools you'll need to get the following dependencies:

 - Download GPLex 1.2.2 [from here](https://s3.eu-central-1.amazonaws.com/nb-stor/dos-legacy/ExportTool/gplex-distro-1_2_2.zip) and extract it to the `External\gplex\` directory
 - Download GPPG 1.5.2 [from here](https://s3.eu-central-1.amazonaws.com/nb-stor/dos-legacy/ExportTool/gppg-distro-1_5_2.zip) and extract it to the `External\gppg\` directory
 - Protocol Buffers 3.6.1 compiler [from here](https://github.com/protocolbuffers/protobuf/releases/download/v3.6.1/protoc-3.6.1-win32.zip) and extract it to the `External\protoc\` directory
