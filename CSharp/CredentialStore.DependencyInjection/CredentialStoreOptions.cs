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
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// CredentialStoreOptions supported by <see cref="ICredentialManager"/>
    /// </summary>
    public readonly struct CredentialStoreOptions : IEquatable<CredentialStoreOptions>
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="CredentialStoreOptions"/> class.
        /// </summary>
        /// <param name="loggerType">Logger Type to be used within injected service</param>
        /// <param name="serviceLifetime">service life time to be used for injected services</param>
        public CredentialStoreOptions(LoggerType loggerType, ServiceLifetime serviceLifetime)
        {
            LoggerType = loggerType;
            ServiceLifetime = serviceLifetime;
        }

        /// <summary>
        /// specifies the <see cref="LoggerType"/> to be used
        /// </summary>
        public LoggerType LoggerType { get; }
        /// <summary>
        /// Specified the <see cref="ServiceLifetime"/> to be used.
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }

        /// <summary>
        /// Deconstructs object into individual properties
        /// </summary>
        public void Deconstruct(out LoggerType loggerType, out ServiceLifetime serviceLifetime)
        {
            loggerType = LoggerType;
            serviceLifetime = ServiceLifetime;
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="first"/> is equal to <paramref name="second"/>;
        /// otherwise <c>false</c>
        /// </summary>
        public static bool operator ==(CredentialStoreOptions first, CredentialStoreOptions second) =>
            first.LoggerType == second.LoggerType &&
            first.ServiceLifetime == second.ServiceLifetime;

        /// <summary>
        /// Returns <c>true</c> if <paramref name="first"/> is not equal to <paramref name="second"/>;
        /// otherwise <c>false</c>
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(CredentialStoreOptions first, CredentialStoreOptions second) =>
            !(first == second);

        /// <summary>
        /// <inheritdoc cref="IEquatable{CredentialStoreOptions}.Equals(CredentialStoreOptions)"/>
        /// </summary>
        public bool Equals(CredentialStoreOptions other) =>
            LoggerType == other.LoggerType && ServiceLifetime == other.ServiceLifetime;
        /// <summary>
        /// <inheritdoc cref="Object.Equals(object?)"/>
        /// </summary>
        public override bool Equals(object? obj) =>
            obj is CredentialStoreOptions other && Equals(other);
        /// <summary>
        /// <inheritdoc cref="Object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hashCode = 0;
                hashCode ^= 397 * LoggerType.GetHashCode();
                hashCode ^= 397 * ServiceLifetime.GetHashCode();
                return hashCode;
            }
#else
            return HashCode.Combine((int) LoggerType, (int) ServiceLifetime);
#endif
        }

    }
}
