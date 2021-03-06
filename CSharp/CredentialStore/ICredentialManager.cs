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
using System.Collections.Generic;
using System.ComponentModel;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Win32 Credential Manager (Credential Repository) providing CRUD 
    /// operations for Windows Credential Manager 
    /// </summary>
    public interface ICredentialManager
    {
        /// <summary>
        /// Returns all Credentials from the Users credential set
        /// </summary>
        IEnumerable<Credential> Credentials { get; }

        /// <summary>
        /// Adds <paramref name="credential"/> to Win32 
        /// Credential Manager
        /// </summary>
        /// <param name="credential">credential to be saved</param>
        /// <returns>true on success; otherwise, false</returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credential"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <see cref="Credential.Id"/> or 
        /// <see cref="Credential.UserName"/> are null or empty
        /// </exception>
        void Add(Credential credential);

        /// <summary>
        /// Updates existing credential with <paramref name="credential"/>
        /// </summary>
        /// <param name="credential">credential to update with values to update</param>
        /// <remarks>
        /// thinly veiled wrapper around <see cref="Add"/> as both do the same write operation,
        /// provided mostly for readability
        /// </remarks>
        void Update(Credential credential);

        /// <summary>
        /// deletes a credential from the user's credential set
        /// </summary>
        /// <param name="credential">credential to be removed</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credential"/> is null
        /// </exception>
        /// <exception cref="Win32Exception">
        /// if error occurs calling native api
        /// </exception>
        void Delete(Credential credential);

        /// <summary>
        /// deletes a credential from the user's credential set
        /// </summary>
        /// <param name="id">id of item to be deleted</param>
        /// <param name="type">credential type of item to be deleted</param>
        /// <exception cref="ArgumentException">
        /// if <paramref name="id"/> is null or empty
        /// </exception>
        /// <exception cref="Win32Exception">
        /// if error occurs calling native api
        /// </exception>
        void Delete(string id, CredentialType type);

        /// <summary>
        /// Finds a credential with the given id value and optionally <see cref="CredentialType"/>
        /// </summary>
        /// <param name="id">id of the credential to be found</param>
        /// <param name="type">
        /// <see cref="CredentialType"/> of <see cref="Credential"/> to be found, 
        /// by default <see cref="CredentialType.DomainPassword"/>
        /// </param>
        /// <returns><see cref="Credential"/> found or null</returns>
        /// <exception cref="ArgumentException">
        /// if <paramref name="id"/> is null or empty or if returned credential structure is malformed
        /// </exception>
        Credential? Find(string id, CredentialType type = CredentialType.Generic);
        /// <summary>
        /// Returns all credentials matching wildcard based <paramref name="filter"/>
        /// </summary>
        /// <param name="filter">filter using wildcards</param>
        /// <param name="searchAll">if true all credentials are searched</param>
        /// <returns><see cref="IEnumerable{Credential}"/> of credentials matching filter</returns>
        IEnumerable<Credential> Find(string filter, bool searchAll);
    }
}
