namespace Vtex.Toolbelt.Model
{
    public class FileConflict
    {
        private const int Kilobyte = 1024;
        private const int Megabyte = Kilobyte*1024;

        public string Path { get; set; }

        public int? LocalSize { get; set; }

        public int? RemoteSize { get; set; }

        public FileConflict(string path, int? localSize, int? remoteSize)
        {
            Path = path;
            LocalSize = localSize;
            RemoteSize = remoteSize;
        }

        public static string HumanizeBytes(int? size)
        {
            if(size == null)
                return "-";

            if (size < Kilobyte)
                return size + " B";

            if (size < Megabyte)
                return ((decimal)size / Kilobyte).ToString("#.#") + " KB";

            return ((decimal)size / Megabyte).ToString("#.#") + " MB";
        }
    }
}
