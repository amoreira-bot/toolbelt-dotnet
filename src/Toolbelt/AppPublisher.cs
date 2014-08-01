namespace Vtex.Toolbelt
{
    using Services;

    public class AppPublisher
    {
        private readonly GalleryClient galleryClient;

        public AppPublisher(string authenticationToken, Configuration configuration)
        {
            this.galleryClient = new GalleryClient("", authenticationToken, configuration.GalleryEndpoint);
        }

        public void PublishApp(string name, string version, byte[] files)
        {
            this.galleryClient.PushApp(name, version, files);
        }
    }
}