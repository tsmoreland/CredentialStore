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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly ITextOutputWriter _writer;
        private readonly IObscruredReader _obscruredReader;
        private readonly ILoggerAdapter _logger;
        private static readonly Lazy<IReadOnlyDictionary<string, string>> _lazyUsage = new Lazy<IReadOnlyDictionary<string,string>>(GetUsageDictionary);

        /// <summary>
        /// Instantiates a new instance of the
        /// <see cref="CredentialExecuter"/> class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credentialManager"/> or
        /// <paramref name="writer"/> or
        /// <paramref name="obscruredReader"/> or
        /// <paramref name="logger"/> are null
        /// </exception>
        public CredentialExecuter(ICredentialManager credentialManager, ITextOutputWriter writer, IObscruredReader obscruredReader, ILoggerAdapter logger)
        {
            _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _obscruredReader = obscruredReader ?? throw new ArgumentNullException(nameof(obscruredReader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public CredentialStoreOperation? GetOperation(string name)
        {
            name ??= string.Empty;

            CredentialStoreOperation? operation = name.ToLowerInvariant() switch
            {
                "add" => Add,
                "list" => List,
                "remove" => Remove,
                _ =>  null,
            };
            if (operation == null)
                _logger.Error($"unsupported operation {name}");
            return operation;
        }

        /// <inheritdoc/>
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
            string secret = _obscruredReader.ReadSecret("password");

            switch (type)
            {
                case CredentialType.Generic:
                    _credentialManager.Add(new Credential(
                        target, 
                        username, 
                        secret, 
                        CredentialFlag.None,
                        CredentialType.Generic, 
                        CredentialPersistence.LocalMachine, 
                        DateTime.Now));
                    break;
                case CredentialType.DomainPassword:
                    _credentialManager.Add(new Credential(
                        target, 
                        username, 
                        secret, 
                        CredentialFlag.None,
                        CredentialType.DomainPassword, 
                        CredentialPersistence.LocalMachine, 
                        DateTime.Now));
                    break;
                default:
                    _logger.Error($"unsupported type {type}");
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Find(Span<string> args)
        {
            if (args.Length < 1)
            {
                _writer.WriteLine(GetUsage());
                return false;
            }

            if (args[0].ToLowerInvariant() == "help")
            {
                _writer.WriteLine(GetUsage());
                return true;
            }

            bool searchAll = args.Length == 1 || args.Length > 1 && args[2]?.ToLowerInvariant() == "all";

            var builder = new StringBuilder();
            _credentialManager.Find(args[0], searchAll)
                .Select(credential => $"{credential.Id} - {credential.UserName}:{credential.Secret}")
                .ToList()
                .ForEach(msg => builder.AppendLine(msg));
            _writer.WriteLine(builder.ToString());

            return true;
        }

        /// <inheritdoc/>
        public bool Remove(Span<string> args)
        {
            if (args.Length < 2)
            {
                _writer.WriteLine(GetUsage());
                return false;
            }

            if (args[0].ToLowerInvariant() == "help")
            {
                _writer.WriteLine(GetUsage());
                return true;
            }

            string target = args[0].ToLowerInvariant();
            IEnumerable<Credential> credentials;
            if (args.Length > 1)
            {
                if (!Enum.TryParse(args[1], true, out CredentialType type))
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
                try
                {
                    _credentialManager.Delete(credential);
                    _writer.WriteLine($"Deleted {credential.Id} {credential.UserName}");
                    return true;
                }
                catch (Win32Exception ex)
                {
                    _writer.WriteLine($"Failed to delete {credential.Id} {credential.UserName}, Error: {ex.NativeErrorCode:x8}");
                    return false;
                }
            });
        }

        /// <inheritdoc/>
        public bool List(Span<string> args)
        {
            var builder = new StringBuilder();
            _credentialManager
                .Credentials
                .Select(credential => $"{credential.Id} - {credential.UserName}:{credential.Secret}")
                .ToList()
                .ForEach(msg => builder.AppendLine(msg));

            _writer.WriteLine(builder.ToString());
            return true;
        }

        private static string GetUsage([CallerMemberName] string callerMemberName = "")
        {
            if (_lazyUsage.Value.ContainsKey(callerMemberName))
                return _lazyUsage.Value[callerMemberName];

            return new StringBuilder()
                .AppendLine("Usage: CredentialStore.Cli <verb>")
                .AppendLine("\tsupported verbs: list, add, remove")
                .ToString();
        }
        private static IReadOnlyDictionary<string, string> GetUsageDictionary() =>
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { nameof(Add), "Usage: CredentialStore.Cli add <type> <target> <username>" },
                { nameof(Remove), "Usage: CredentialStore.Cli remove <target> (<type>)" },
                { nameof(Find), "Usage: CredentialStore.Cli find <filter> (<search all, defaults true>)" },
                { nameof(List), "Usage: CredentialStore.Cli list" },
            });

    }
}
