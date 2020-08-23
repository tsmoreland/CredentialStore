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
using Moreland.Security.Win32.CredentialStore;
using Moreland.Security.Win32.CredentialStore.NativeApi;

// ReSharper disable once CheckNamespace -- intentional namespace to simplify use with IServiceProvider
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CredentialStoreServiceCollectionExtensionMethods
    {
        /// <summary>
        /// Adds <see cref="ICredentialManager"/> to <paramref name="serviceCollection"/> along with any required
        /// services it needs
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="options">options used to determine lifetime, and which logger is used</param>
        /// <example>
        /// <code>
        ///  public void ConfigureServices(IServiceCollection services)
        ///  {
        ///      services.AddCredentialStore(ServiceLifetime.Singleton);
        ///  }
        /// </code>
        /// </example>
        public static IServiceCollection AddCredentialStore(IServiceCollection serviceCollection, CredentialStoreOptions options)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            var (loggerType, serviceLifetime) = options;

            switch (loggerType)
            {
                case LoggerType.CustomLogger:
                    // setup by externally, no longer our responsibility
                    break;
                case LoggerType.None:
                    serviceCollection.AddSingleton<ILoggerAdapter, NullLoggerAdapter>();
                    break;
                case LoggerType.MsExtensionsLogger:
                    serviceCollection.AddSingleton<ILoggerAdapter, MsExtensionsLoggerAdapter>();
                    break;
                case LoggerType.TraceSwitchLogger:
                    serviceCollection.AddSingleton<ILoggerAdapter, TraceLoggerAdapter>();
                    break;
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped<ICredentialManager, CredentialManager>();
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient<ICredentialManager, CredentialManager>();
                    break;
                default: // singleton
                    serviceCollection.AddSingleton<ICredentialManager, CredentialManager>();
                    break;
            }

            serviceCollection
                .AddSingleton<INativeInterop, INativeInterop>()
                .AddSingleton<ICriticalCredentialHandleFactory, CriticalCredentialHandleFactory>()
                .AddSingleton<IErrorCodeToStringService, ErrorCodeToStringService>();

            return serviceCollection;
        }

    }
}
