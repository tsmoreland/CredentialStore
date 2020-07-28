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
using System.IO;
using System.Text;

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    /// <summary>
    /// <inheritdoc cref="IObscruredReader"/>
    /// </summary>
    public sealed class ConsoleObscuredReader : IObscruredReader
    {
        private readonly IConsoleInputReader _reader;
        private readonly ITextOutputWriter _writer;
        private readonly ILoggerAdapter _logger;

        /// <summary>
        /// Instantiates a new instance of the
        /// <see cref="ConsoleObscuredReader"/> class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if any of the provided arguments are null
        /// </exception>
        public ConsoleObscuredReader(IConsoleInputReader reader, ITextOutputWriter writer, ILoggerAdapter logger)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// <see cref="IObscruredReader.ReadSecret(string)"/>
        /// </summary>
        public string ReadSecret(string prompt)
        {
            try
            {
                if (!string.IsNullOrEmpty(prompt))
                    _writer.Write($"{prompt}: ");
                ConsoleKey key;
                var builder = new StringBuilder();
                do
                {
                    char keyChar;
                    (key, keyChar) = _reader.ReadKey(true);
                    if (key != ConsoleKey.Backspace && key != ConsoleKey.Enter)
                    {
                        builder.Append(keyChar);
                        _writer.Write("*");
                    }
                    else if (key == ConsoleKey.Backspace && builder.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                        _writer.Write("\b \b");
                    }

                } while (key != ConsoleKey.Enter);

                return builder.ToString();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is IOException || ex is ObjectDisposedException)
            {
                _logger.Error(ex.Message, ex);
                return string.Empty;
            }
        }
    }
}
