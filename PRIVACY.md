# Privacy

Robocopy Drop contains no telemetry, analytics, advertising, account system,
or background resident agent.

When update checks are enabled, the application sends an HTTPS request to the
public GitHub Releases API for the configured repository. The request contains
the standard network metadata visible to GitHub and a user-agent containing
the installed Robocopy Drop version. No file names, copied paths, reports, or
copy statistics are sent.

Update checks run at most once every 24 hours when a Robocopy Drop window is
opened. They can be disabled in **Impostazioni Robocopy Drop**. A manual check
is also available.

An update is downloaded only after the user confirms. The MSI is saved under
`%LOCALAPPDATA%\RobocopyDrop\Updates`, checked against the SHA-256 digest
published by GitHub, and then passed to Windows Installer. Depending on the
release configuration, a trusted Authenticode signature may also be required.
