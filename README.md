# Robocopy Drop

Robocopy Drop adds **Copia qui con Robocopy / Copy here via Robocopy** to the
right-button drag-and-drop menu in Windows File Explorer. It provides a
Windows-style progress window, adaptive Robocopy threads, conflict handling,
reports, and optional SHA-256 verification.

Version 1.6.0 adds GitHub-based update notification and assisted automatic
updates while keeping the Classic architecture: there is no resident agent,
startup process, global hotkey, or persistent queue.

## Main features

- Right-drag files or folders and choose the Robocopy Drop command.
- Adaptive thread profile: local disks, USB/removable media, network, and
  optical media are handled differently.
- Manual 1, 4, 8, 16, 32, or 64-thread selection.
- Replace, skip, or keep both on conflicts.
- Expandable technical details, saved reports, and SHA-256 verification.
- Italian and English MSI packages.
- Per-user or per-machine installation.
- Direct **Uninstall Robocopy Drop** Start-menu shortcut.
- Update checks through public GitHub Releases.

## Update behavior

Automatic checks are enabled by default only when a GitHub repository is
configured at build time. A check runs at most once every 24 hours when a
Robocopy Drop window opens. It never interrupts an active copy or verification:
the update link is shown when the operation is idle or completed.

The user must confirm before download and installation. The updater:

1. reads the latest stable, non-draft GitHub Release;
2. selects the exact MSI for the installed UI language;
3. downloads only over HTTPS from GitHub release hosts;
4. validates the asset size and GitHub-provided SHA-256 digest;
5. validates Authenticode when present, and can require a trusted signature
   and allowlisted signer thumbprint;
6. starts Windows Installer with `/passive /norestart`.

No files, paths, or copy statistics are sent. See [PRIVACY.md](PRIVACY.md).

## Configure the GitHub repository

Before creating a public release, run:

```text
Configura-GitHub.cmd
```

Enter the GitHub user or organization and repository name. The result is saved
to `github-release.json` and embedded in the runner config during the MSI build.

Without an owner, the MSI still builds, but update checks remain disabled.

## Build locally

Requirements:

- Windows 10 or 11 x64;
- .NET Framework 4.8 build tools / C# compiler;
- .NET SDK 8;
- internet access for the first WiX restore;
- optional Windows SDK and Authenticode code-signing certificate.

Run:

```text
Crea-MSI.cmd
```

The script displays the WiX/OSMF notice and asks for explicit acceptance. The
release folder contains language-specific MSI files, manuals, checksums, and a
release manifest.

The C# runner is compiled from source on every build. The Classic shell
extension DLLs are the tested x64 baseline and are SHA-256 locked; the
bilingual C++ source and manual rebuild commands are documented in
`src/BUILD-SHELL-EXTENSION.txt`.

## GitHub release workflow

The included `.github/workflows/release.yml` builds on a version tag such as
`v1.6.0`. Before using it:

1. set the repository variable `WIX_OSMF_ACCEPTED` to `true` only after checking
   the current WiX terms;
2. optionally configure `WINDOWS_SIGNING_PFX_BASE64` and
   `WINDOWS_SIGNING_PFX_PASSWORD` secrets;
3. push a version tag.

The workflow writes the current repository coordinates into
`github-release.json`, builds both MSI packages, and publishes the release
assets. Review the first workflow run and release before enabling immutable
releases.

## Release asset naming

The updater expects exactly:

```text
RobocopyDrop-X.Y.Z-it-x64.msi
RobocopyDrop-X.Y.Z-en-x64.msi
```

The release tag may be `vX.Y.Z`.

## License

Robocopy Drop is released under the [MIT License](LICENSE).

Robocopy is included with Windows and is not redistributed. WiX is a build
tool restored from NuGet and remains subject to its own terms. See
[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt) and
[docs/NOTICE-WIX.txt](docs/NOTICE-WIX.txt).

## Security

For public distribution, signed MSI and executable files are strongly
recommended. See [SECURITY.md](SECURITY.md).
