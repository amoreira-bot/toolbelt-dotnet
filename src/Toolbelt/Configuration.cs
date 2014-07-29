namespace Vtex.Toolbelt
{
    public class Configuration
    {
        public const int DefaultFileSystemDelay = 300;
        public const string DefaultGalleryEndpoint = "http://gallery.vtexcommercebeta.com.br/api/gallery/";

        public int FileSystemDelay { get; set; }

        public string GalleryEndpoint { get; set; }

        public Configuration()
        {
            this.FileSystemDelay = DefaultFileSystemDelay;
            this.GalleryEndpoint = DefaultGalleryEndpoint;
        }
    }
}