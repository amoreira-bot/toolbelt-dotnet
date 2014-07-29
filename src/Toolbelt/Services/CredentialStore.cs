using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class CredentialStore
    {
        private static readonly string FilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "credentials.json");
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public Credential GetCurrent()
        {
            if (!File.Exists(FilePath))
                return null;

            var json = File.ReadAllText(FilePath, DefaultEncoding);
            return JsonConvert.DeserializeObject<Credential>(json);
        }

        public void Save(Credential credential)
        {
            var json = JsonConvert.SerializeObject(credential, Formatting.Indented);
            File.WriteAllText(FilePath, json, DefaultEncoding);
        }
    }
}