# CredentialStore

Win32 Credential Manager API wrapping Win32 funcitons

## Build Status

![C# Continuous Integration](https://github.com/tsmoreland/CredentialStore/workflows/C%23%20Continuous%20Integration/badge.svg)

![C++ Continuous Integration](https://github.com/tsmoreland/CredentialStore/workflows/C++%20Continuous%20Integration/badge.svg)

## Credential Manager

provides Credential Manager allowing CRUD operations to Win32 Credential Manager

## Credential

Managed wrapper around Win32 CREDENTIAL structure, secrets are stored as string so no security is guarenteed at this time it is expected that the machine itself and specifically the process will be responsible for this security.
