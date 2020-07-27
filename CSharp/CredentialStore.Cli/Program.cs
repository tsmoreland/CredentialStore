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
using Microsoft.Extensions.Logging;

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: CredentialStore.Cli <verb>");
                return;
            }

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

                var executer = provider.GetService<ICredentialExecuter>();
                var remainingArgs = new Span<string>(args, 1, args.Length - 1);

                switch (args[0].ToLowerInvariant())
                {
                    case "add":
                        executer.Add(remainingArgs);
                        break;
                    case "delete":
                        executer.Delete(remainingArgs);
                        break;
                    case "list":
                        executer.List(remainingArgs);
                        break;
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {
                // ... ignore error ...
            }
        }
    }
}
