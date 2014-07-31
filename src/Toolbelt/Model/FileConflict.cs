namespace Vtex.Toolbelt.Model
{
    public class FileConflict
    {
        public string Path { get; set; }

        public int? LocalSize { get; set; }

        public int? RemoteSize { get; set; }

        public FileConflict(string path, int? localSize, int? remoteSize)
        {
            Path = path;
            LocalSize = localSize;
            RemoteSize = remoteSize;
        }
    }
}
