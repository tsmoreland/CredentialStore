//
// Copyright © 2020 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 


using System;
using System.ComponentModel;
using System.Collections.Generic;
using NativeCredential = Moreland.Security.Win32.CredentialStore.NativeApi.Credential;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Wrapper around Native Credential Management functions which return the results to managed objects
    /// and/or errors as exceptions.
    /// </summary>
    public interface INativeInterop
    {
        /// <summary>
        /// deletes a credential from the users credential set
        /// </summary>
        /// <param name="target">
        /// Pointer to a null-terminated string that contains the name of the
        /// credential to delete.
        /// </param>
        /// <param name="type">
        /// Type of the credential to delete. Must be one of the
        /// <see cref="CredentialType"/> defined types. For a list of the
        /// defined types, see the Type member of the <see cref="NativeCredential"/>
        /// structure.
        /// </param>
        /// <param name="flags">Reserved and must be zero.</param>
        /// <exception cref="Win32Exception">on native api error</exception>
        void CredDelete(string target, int type, int flags);

        /// <summary>
        /// Returns an <see cref="IEnumerable{NativeCredential}"/>
        /// matching <paramref name="filter"/> if non-null; otherwise
        /// unfilteredjj
        /// </summary>
        /// <param name="filter">
        /// Pointer to a null-terminated string that contains the filter for
        /// the returned credentials. Only credentials with a TargetName
        /// matching the filter will be returned. The filter specifies a name
        /// prefix followed by an asterisk. For instance, the filter "FRED*"
        /// will return all credentials with a TargetName beginning with the
        /// string "FRED".
        /// If null is specified, all credentials will be returned.
        /// </param>
        /// <param name="flag">
        /// The value of this parameter can be zero or more of
        /// <see cref="NativeApi.EnumerateFlag"/> values combined with a bitwise-OR
        /// operation.
        /// </param>
        /// <returns><see cref="IEnumerable{NativeCredential}"/></returns>
        /// <exception cref="Win32Exception">
        /// if unable to enumerate over credentials, retrieve a credential or
        /// free one that has been retrieved
        /// </exception>
        IEnumerable<NativeCredential> CredEnumerate(string? filter, int flag);
        /// <summary>
        /// Frees the buffer allocated by any of the credentials api functions.
        /// </summary>
        /// <param name="handle">Pointer to the buffer to be freed.</param>
        /// <exception cref="Win32Exception">on native api error</exception>
        void CredFree(IntPtr handle);
        /// <summary>
        /// Returns <see cref="NativeCredential"/> containing the matching
        /// credential if found; otherwise, null
        /// </summary>
        /// <param name="target">
        /// Pointer to a null-terminated string that contains the name of the
        /// credential to read.
        /// </param>
        /// <param name="type">
        /// Type of the credential to read. Type must be one of the
        /// <see cref="CredentialType"/> defined types.
        /// </param>
        /// <param name="reservedFlag">Currently reserved and must be zero.</param>
        /// <returns>
        /// <see cref="NativeCredential"/> containing the matching credential if found;
        /// otherwise, null
        /// </returns>
        /// <exception cref="Win32Exception">on native api error</exception>
        NativeCredential? CredRead(string target, CredentialType type, int reservedFlag);
        /// <summary>
        /// Creates or updates a credential
        /// </summary>
        /// <param name="credential">
        /// A pointer to the <see cref="Credential"/> structure to be written.
        /// </param>
        /// <param name="flags">
        /// Flags that control the function's operation.
        /// <see cref="NativeApi.PreserveFlag"/> for valid values.
        /// </param>
        /// <exception cref="Win32Exception">on native api error</exception>
        void CredWrite(NativeApi.Credential credential, int flags);
    }
}
