using System.Collections.Generic;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public interface IFileSystem
    {
        string CurrentDirectory { get; }

        string ReadTextFile(string relativePath);

        IEnumerable<FileState> GetFileStates();

        void DeleteFile(string path);

        void WriteFile(string path, byte[] file);
    }
}