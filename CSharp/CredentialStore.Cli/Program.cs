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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: CredentialStore.Cli <verb>");
                return 1;
            }

            ILoggerAdapter? logger = null;
            try
            {
                var services = new ServiceCollection();
                services
                    .AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Information);
                        builder.AddConsole();
                    })
                    .AddScoped<ICredentialManager, CredentialManager>()
                    .AddScoped<ILoggerAdapter, ConsoleLoggingAdapter>()
                    .AddScoped<ICredentialExecuter, CredentialExecuter>();
                using var provider = services.BuildServiceProvider();

                logger = provider.GetService<ILoggerAdapter>();
                if (logger == null)
                    throw new KeyNotFoundException("unable to get logger");

                var executer = provider.GetService<ICredentialExecuter>();
                return executer
                    .GetOperation(args[0])?
                    .Invoke(new Span<string>(args, 1, args.Length - 1)) == true 
                    ? 0 
                    : 2;
            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {
                logger?.Error(ex.Message, ex);
                return 3;
            }
        }
    }
}