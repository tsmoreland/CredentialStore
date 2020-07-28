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
using System.Linq;
using System.Text;

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    /// <summary>
    /// delegate for operations provided by <see cref="ICredentialExecuter"/>
    /// </summary>
    /// <param name="arguments">arguments used by each operation</param>
    /// <returns>true if operation was successful; otherwise, false</returns>
    public delegate bool CredentialStoreOperation(Span<string> arguments);

    /// <summary>
    /// <inheritdoc cref="ICredentialExecuter"/>
    /// </summary>
    public sealed class CredentialExecuter : ICredentialExecuter
    {
        private readonly ICredentialManager _credentialManager;
        private readonly ILoggerAdapter _logger;

        /// <summary>
        /// Instantiates a new instance of the
        /// <see cref="CredentialExecuter"/> class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credentialManager"/> or
        /// <paramref name="logger"/> are null
        /// </exception>
        public CredentialExecuter(ICredentialManager credentialManager, ILoggerAdapter logger)
        {
            _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// <see cref="ICredentialExecuter.GetOperation(string)"/>
        /// </summary>
        public CredentialStoreOperation? GetOperation(string name)
        {
            CredentialStoreOperation? operation = name?.ToLowerInvariant() switch
            {
                "add" => Add,
                "list" => List,
                "delete" => Delete,
                _ =>  null,
            };
            if (operation == null)
                _logger.Error($"unsupported operation {name}");
            return operation;
        }

        /// <summary>
        /// <see cref="ICredentialExecuter.Add(Span{string})"/>
        /// </summary>
        public bool Add(Span<string> args)
        {
            if (args.Length < 3)
            {
                _logger.Error("Usage: CredentialStore.Cli add <type> <target> <username>");
                return false;
            }

            if (!Enum.TryParse(args[0], true, out CredentialType type))
            {
                _logger.Error($"unrecognized type {args[0]}");
                return false;
            }

            string target = args[1];
            string username = args[2];
            const string secret = "password"; // TODO: secure read from console

            switch (type)
            {
                case CredentialType.Generic:
                    _credentialManager.Add(new Credential(
                        target, 
                        username, 
                        secret, 
                        CredentialFlag.None,
                        CredentialType.Generic, 
                        CredentialPeristence.LocalMachine, 
                        DateTime.Now));
                    break;
                case CredentialType.DomainPassword:
                    _credentialManager.Add(new Credential(
                        target, 
                        username, 
                        secret, 
                        CredentialFlag.None,
                        CredentialType.DomainPassword, 
                        CredentialPeristence.LocalMachine, 
                        DateTime.Now));
                    break;
                default:
                    _logger.Error($"unsupported type {type}");
                    return false;
            }

            return true;
        }
        /// <summary>
        /// <see cref="ICredentialExecuter.Delete(Span{string})"/>
        /// </summary>
        public bool Delete(Span<string> args)
        {
            if (args.Length < 1)
            {
                // TODO: log usage, preferable from dictionary
                return false;
            }

            if (args[0].ToLowerInvariant() == "help")
            {
                // TODO: output usage
                return true;
            }

            string target = args[1].ToLowerInvariant();
            IEnumerable<Credential> credentials;
            if (args.Length > 1)
            {
                if (!Enum.TryParse(args[2], true, out CredentialType type))
                {
                    _logger.Error($"Unrecognized or unsupported type '{args[2]}'");
                    return false;
                }

                var credential = _credentialManager.Find(target, type);
                credentials = credential != null
                    ? new[] {credential}
                    : Enumerable.Empty<Credential>();
            }
            else
                credentials = _credentialManager
                    .Credentials
                    .Where(c => c.Id.ToUpperInvariant() == target)
                    .ToArray();


            return credentials.All(credential =>
            {
                if (_credentialManager.Delete(credential))
                {
                    _logger.Info($"Deleted {credential.Id} {credential.UserName}");
                    return true;
                }
                else
                {
                    _logger.Error($"Failed to delete {credential.Id} {credential.UserName}");
                    return false;
                }
            });
        }

        /// <summary>
        /// <see cref="ICredentialExecuter.List(Span{string})"/>
        /// </summary>
        public bool List(Span<string> args)
        {
            var builder = new StringBuilder();
            _credentialManager
                .Credentials
                .Select(credential => $"{credential.Id} - {credential.UserName}:{credential.Secret}")
                .ToList()
                .ForEach(msg => builder.AppendLine(msg));

            _logger.Info(builder.ToString());
            return true;
        }

        /*
            sample code to read from console without printing, or rather overwriting characters

            ConsoleKeyInfo keyInfo;
         
            do
            {
                keyInfo = Console.ReadKey(true);
                // Skip if Backspace or Enter is Pressed
                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        // Remove last charcter if Backspace is Pressed
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("b b");
                    }
                }
            }
            // Stops Getting Password Once Enter is Pressed
            while (keyInfo.Key != ConsoleKey.Enter);
         */
    }
}
