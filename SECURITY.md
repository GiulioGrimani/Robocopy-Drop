# Security policy

## Supported versions

Security fixes are provided for the latest published release.

## Reporting a vulnerability

Do not open a public issue for a vulnerability that could put users at risk.
Use GitHub's private vulnerability reporting feature in the repository
Security settings.

Include the affected version, reproduction steps, impact, and any proposed
mitigation. Please do not include personal data or files copied by users.

## Release security

Official release assets are published through GitHub Releases. The built-in
updater accepts only the exact language-specific MSI name for the newer
version, requires HTTPS, validates the SHA-256 digest returned by GitHub, and
can require an Authenticode signature and an allowlisted signer thumbprint.

For public distribution, sign the runner, shell-extension DLLs, and MSI with
a trusted code-signing certificate. Enable GitHub immutable releases after
the release process has been validated.
