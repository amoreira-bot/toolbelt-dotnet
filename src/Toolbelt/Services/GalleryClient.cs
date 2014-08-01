using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Vtex.Toolbelt.Model;
using Newtonsoft.Json;

namespace Vtex.Toolbelt.Services
{
    public class GalleryClient
    {
        private readonly string _rootPath;
        private readonly HttpClient _httpClient;

        public GalleryClient(string rootPath, string authenticationToken, string endpointUrl)
        {
            _rootPath = rootPath;

            _httpClient = new HttpClient {BaseAddress = new Uri(endpointUrl)};
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", authenticationToken);
        }

        public void SendWorkspaceChanges(string accountName, string workspace, IEnumerable<Change> changes, bool resync)
        {
            var payload = GetPayloadFor(changes, "File changes via VTEX Toolbelt");
            var path = string.Format("accounts/{0}/{1}/changes", accountName, workspace);
            SendChanges(path, payload, resync);
        }

        public void SendSandboxChanges(string appName, IEnumerable<Change> changes, bool resync)
        {
            var payload = GetPayloadFor(changes);
            var path = string.Format("sandbox/{0}/changes", appName);
            SendChanges(path, payload, resync);
        }

        private ChangeBatchRequest GetPayloadFor(IEnumerable<Change> changes, string message = null)
        {
            var payload = new ChangeBatchRequest
            {
                Changes = changes.Select(change => ChangeRequest.FromChange(change, _rootPath)).ToArray(),
                Message = message,
            };

            return payload;
        }

        private void SendChanges(string path, ChangeBatchRequest payload, bool resync)
        {
            if (resync)
            {
                path += "?resync=true";
            }

            var response = _httpClient.PostAsync(path, payload, new JsonMediaTypeFormatter()).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(string.Format("Failed to send changes with status code {0} ({1})",
                    (int) response.StatusCode, response.StatusCode), response);
            }
        }

        public void PushApp(string name, string version, byte[] files)
        {
            var requestContent = new MultipartFormDataContent();
            var zipContent = new ByteArrayContent(files);

            var fileName = string.Format("{0}-{1}", name, version);

            zipContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
            requestContent.Add(zipContent, fileName, fileName + ".zip");

            var response = _httpClient.PostAsync("apps", requestContent).Result;

            if (!response.IsSuccessStatusCode)
            {
                dynamic body = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                throw new ApiException(string.Format("Failed to publish app with status code {0} ({1})",
                    (int) response.StatusCode, body.Message), response);
            }
        }
    }
}