namespace Vtex.Toolbelt.Core
{
    public class Configuration
    {
        public int FileSystemDelay { get; set; }

        public string GalleryEndpoint { get; set; }

        public Configuration()
        {
            this.FileSystemDelay = 300;
            this.GalleryEndpoint = "http://gallery.vtexcommercebeta.com.br/api/gallery/";
        }
    }
}