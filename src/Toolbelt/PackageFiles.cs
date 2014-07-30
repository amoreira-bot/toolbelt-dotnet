using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

namespace Vtex.Toolbelt
{
    public class PackageFiles
    {
        private readonly string _root;

        public PackageFiles(string root)
        {
            _root = root;
        }

        public byte[] Compress(IEnumerable<string> files)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    foreach (var file in files)
                    {
                        zip.AddFile(file).FileName = this.RelativeFilePath(file);
                    }
                    zip.Save(ms);
                }
                return ms.ToArray();
            }
        }

        private string RelativeFilePath(string file)
        {
            return file.Substring(_root.Length + 1);
        }
    }
}