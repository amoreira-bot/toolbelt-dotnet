using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class PhysicalFileSystem : IFileSystem
    {
        private readonly string _root = Environment.CurrentDirectory;
        private static readonly HashAlgorithm HashAlgorithm = MD5.Create();
        private static readonly string[] BinaryExtensions = {"gz", "pdf", "woff", "zip", "gif", "jpeg", "jpg", "png"};

        public string CurrentDirectory
        {
            get { return _root; }
        }

        public string ReadTextFile(string relativePath)
        {
            var filePath = Path.Combine(_root, relativePath);
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        public IEnumerable<FileState> GetFileStates()
        {
            return Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories)
                .Select(GetFileState);
        }

        private FileState GetFileState(string path)
        {
            var content = IsBinary(path)
                ? File.ReadAllBytes(path)
                : NormalizeLineEndings(File.ReadAllText(path, Encoding.UTF8));

            return new FileState
            {
                Hash = ComputeHash(content),
                Size = content.Length,
                Path = NormalizePath(path)
            };
        }

        private static string ComputeHash(byte[] content)
        {
            return HashAlgorithm.ComputeHash(content).Aggregate("", (acc, current) => acc + current.ToString("x2"));
        }

        private static bool IsBinary(string path)
        {
            return BinaryExtensions.Any(extension =>
                path.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase));
        }

        private static byte[] NormalizeLineEndings(string text)
        {
            text = text.Replace("\\r\\n", "\\n");
            return Encoding.UTF8.GetBytes(text);
        }

        private string NormalizePath(string path)
        {
            return path.Substring(_root.Length + 1).Replace('\\', '/');
        }
    }
}