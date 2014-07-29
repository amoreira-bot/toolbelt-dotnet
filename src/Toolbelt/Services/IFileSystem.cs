namespace Vtex.Toolbelt.Services
{
    public interface IFileSystem
    {
        string CurrentDirectory { get; }

        string ReadTextFile(string relativePath);
    }
}