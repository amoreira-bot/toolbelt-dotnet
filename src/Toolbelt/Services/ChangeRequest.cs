using System;
using System.IO;

namespace Vtex.Toolbelt.Services
{
    public class ChangeRequest
    {
        public string Action { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; }

        public static ChangeRequest FromChange(Change change, string rootPath)
        {
            return change.Action == ChangeAction.Update
                ? ForUpdate(change.Path, rootPath)
                : ForDeletion(change.Path);
        }

        private static ChangeRequest ForUpdate(string path, string rootPath)
        {
            return new ChangeRequest
            {
                Action = "update",
                Path = path,
                Encoding = "base64",
                Content = ReadFileAsBase64(System.IO.Path.Combine(rootPath, path))
            };
        }

        private static ChangeRequest ForDeletion(string path)
        {
            return new ChangeRequest
            {
                Action = "delete",
                Path = path
            };
        }

        private static string ReadFileAsBase64(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
    }
}