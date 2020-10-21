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

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    /// <summary>
    /// Command Line interface request executor
    /// </summary>
    public interface ICredentialExecuter
    {
        /// <summary>
        /// Fetches a deletegate to one of the supported operations
        /// </summary>
        /// <param name="name">
        /// one of the supported methods of <see cref="ICredentialExecuter"/>
        /// </param>
        /// <returns>
        /// <see cref="CredentialStoreOperation"/> if operation found;
        /// or null if not found
        /// </returns>
        CredentialStoreOperation? GetOperation(string name);
        /// <summary>
        /// Adds a new credential using the supplied arguments
        /// </summary>
        /// <param name="args">
        /// either type id username with password being prompted
        /// or help which will detail the required arguments
        /// </param>
        /// <returns>
        /// true if credential was added or help was requested;
        /// otherwise false
        /// </returns>
        bool Add(Span<string> args);

        /// <summary>
        /// Deletes a credential matching the supplied arguments
        /// </summary>
        /// <param name="args">
        /// ...pending...
        /// or help which will detail the required arguments
        /// </param>
        /// <returns>
        /// true if credential exists and is removed, or doesn't exist
        /// or using help; otherwise, false
        /// </returns>
        bool Remove(Span<string> args);

        /// <summary>
        /// Searches for credentials match a given filter
        /// </summary>
        /// <param name="args">
        /// args containing filter, and optional second argument,
        /// if second argument is not 'all' then search all flag
        /// is not set when calling native api
        /// </param>
        /// <returns>
        /// if <paramref name="args"/> contains at least one value then true;
        /// otherwise, false
        /// </returns>
        bool Find(Span<string> args);
        /// <summary>
        /// prints out all credentials for the current user
        /// </summary>
        /// <param name="args">
        /// ...pending...
        /// or help which will detail the required arguments
        /// </param>
        /// <returns>
        /// true
        /// </returns>
        bool List(Span<string> args);
    }
}
