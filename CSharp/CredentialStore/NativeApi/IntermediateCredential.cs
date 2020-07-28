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
using System.Runtime.InteropServices;
using System.Text;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// Managed Wrapper around <see cref="Credential"/> managing the memory allocated
    /// to created to convert from <see cref="CredentialStore.Credential"/>
    /// </summary>
    internal sealed class IntermediateCredential : IDisposable
    {
        private readonly IntPtr _credentialBlob;

        /// <summary>
        /// Instantiates a new instance of the 
        /// <see cref="IntermediateCredential"/> class.
        /// </summary>
        /// <param name="credential">
        /// <see cref="CredentialStore.Credential"/> to be converted to 
        /// <see cref="Credential"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credential"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <see cref="CredentialStore.Credential.Id"/> or 
        /// <see cref="CredentialStore.Credential.UserName"/> are null or empty
        /// </exception>
        public IntermediateCredential(CredentialStore.Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            if (string.IsNullOrEmpty(credential.Id))
                throw new ArgumentException("Id cannot be empty");
            if (string.IsNullOrEmpty(credential.UserName))
                throw new ArgumentException("UserName cannot be empty");

            int credentialSize = 0;
            if (string.IsNullOrEmpty(credential.Secret))
                _credentialBlob =  IntPtr.Zero;
            else
            {
                _credentialBlob = Marshal.StringToCoTaskMemUni(credential.Secret);
                credentialSize = Encoding.Unicode.GetBytes(credential.Secret).Length;
            }

            NativeCredential = new Credential
            {
                UserName = string.IsNullOrEmpty(credential.UserName) ? null : credential.UserName,
                TargetName = credential.Id,
                Comment = string.Empty,
                CredentialBlob = _credentialBlob,
                CredentialBlobSize = credentialSize,
                Type = (int)credential.Type,
                Persist = (int)credential.PeristenceType,
            };
        }

        public Credential NativeCredential { get; }

        #region IDisposable
        ///<summary>Finalize</summary>
        ~IntermediateCredential() => Dispose(false);

        ///<summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        ///<summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        ///<param name="disposing">if <c>true</c> then release managed resources in addition to unmanaged</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // no managed memory to release
            }

            if (_credentialBlob != IntPtr.Zero)
                Marshal.FreeCoTaskMem(_credentialBlob);

            _disposed = true;
        }
        #endregion

    }
}
