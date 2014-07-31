using System;
using System.IO;

namespace Vtex.Toolbelt
{
    public interface IConsole
    {
        TextWriter Out { get; }

        void Write(string format, params object[] args);

        void Write(string content);

        void WriteLine(string format, params object[] args);

        void WriteLine(string content);

        void WriteLine();

        string ReadLine(string message);

        string ReadPassword(string message);

        char Prompt(string message, Action<PromptConfiguration> configure);
    }
}