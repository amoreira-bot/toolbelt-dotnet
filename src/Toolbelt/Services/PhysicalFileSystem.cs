using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class PhysicalFileSystem : IFileSystem
    {
        private readonly string _root = Environment.CurrentDirectory;
        private static readonly HashAlgorithm HashAlgorithm = MD5.Create();
        private static readonly string[] BinaryExtensions = {"gz", "pdf", "woff", "zip", "gif", "jpeg", "jpg", "png"};
        private static readonly Encoding Encoding = Encoding.UTF8;

        public string CurrentDirectory
        {
            get { return _root; }
        }

        public string ReadTextFile(string relativePath)
        {
            var filePath = Path.Combine(_root, relativePath);
            return File.ReadAllText(filePath, Encoding);
        }

        public IEnumerable<FileState> GetFileStates()
        {
            return Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories)
                .Select(GetFileState);
        }

        public string GetRelativePath(string fullPath)
        {
            return fullPath.Substring(_root.Length + 1).Replace('\\', '/');
        }

        public bool IsBinary(string path)
        {
            return BinaryExtensions.Any(extension =>
                path.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase));
        }

        public byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(Path.Combine(_root, path));
        }

        public string ReadNormalizedText(string path)
        {
            var text = File.ReadAllText(Path.Combine(_root, path));
            return text.Replace("\r\n", "\n");
        }

        public void DeleteFile(string path)
        {
            File.Delete(Path.Combine(_root, path));
        }

        public void WriteFile(string path, byte[] file)
        {
            var fullPath = Path.Combine(_root, path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllBytes(fullPath, file);
        }

        private FileState GetFileState(string path)
        {
            var content = IsBinary(path) ? ReadBytes(path) : Encoding.GetBytes(ReadNormalizedText(path));

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

        private string NormalizePath(string path)
        {
            return path.Substring(_root.Length + 1).Replace('\\', '/');
        }
    }
}