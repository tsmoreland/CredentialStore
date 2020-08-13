// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Needs initialized to prevent warnign about use before initialized", Scope = "member", Target = "~M:Moreland.Security.Win32.CredentialStore.NativeApi.NativeInterop.CredWrite(Moreland.Security.Win32.CredentialStore.NativeApi.Credential,System.Int32)")]
