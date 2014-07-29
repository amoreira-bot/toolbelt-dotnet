using System;
using System.IO;
using System.Text;

namespace Vtex.Toolbelt.Services
{
    public class PhysicalFileSystem : IFileSystem
    {
        public string CurrentDirectory
        {
            get { return Environment.CurrentDirectory; }
        }

        public string ReadTextFile(string relativePath)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, relativePath);
            return File.ReadAllText(filePath, Encoding.UTF8);
        }
    }
}