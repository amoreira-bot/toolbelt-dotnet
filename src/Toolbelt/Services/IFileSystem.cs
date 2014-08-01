using System.Collections.Generic;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public interface IFileSystem
    {
        string CurrentDirectory { get; }

        string ReadTextFile(string relativePath);

        IEnumerable<FileState> GetFileStates();

        string GetRelativePath(string path);

        bool IsBinary(string path);

        byte[] ReadBytes(string path);

        string ReadNormalizedText(string path);

        void DeleteFile(string path);

        void WriteFile(string path, byte[] file);
    }
}