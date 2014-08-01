using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services.Requests;
using Vtex.Toolbelt.Services.Responses;

namespace Vtex.Toolbelt.Services
{
    public class GalleryClient
    {
        private readonly HttpClient _httpClient;

        public GalleryClient(string authenticationToken, string endpointUrl)
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(endpointUrl)};
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", authenticationToken);
        }

        public void SendWorkspaceChanges(string accountName, string workspace, IEnumerable<FinalizedChange> changes,
            bool resync)
        {
            var payload = GetPayloadFor(changes, "File changes via VTEX Toolbelt");
            var path = string.Format("accounts/{0}/workspaces/{1}/changes", accountName, workspace);
            SendChanges(path, payload, resync);
        }

        public void SendSandboxChanges(string appName, IEnumerable<FinalizedChange> changes, bool resync)
        {
            var payload = GetPayloadFor(changes);
            var path = string.Format("sandbox/{0}/changes", appName);
            SendChanges(path, payload, resync);
        }

        public void CreateWorkspace(string accountName, string workspace)
        {
            var payload = new CreateWorkspaceRequest(workspace);
            var path = string.Format("accounts/{0}/workspaces", accountName);
            var response = Post(path, payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsAsync<ErrorResponse>().Result;
                throw new ApiException(string.Format("{0}: {1}", response.StatusCode.ToString(), error.Message),
                    response);
            }
        }

        public Dictionary<string, FileStateResponse> GetWorkspaceState(string accountName, string workspace)
        {
            var path = string.Format("accounts/{0}/workspaces/{1}/index", accountName, workspace);
            var response = _httpClient.GetAsync(path).Result;

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsAsync<ErrorResponse>().Result;
                throw new ApiException(string.Format("{0}: {1}", response.StatusCode, error.Message), response);
            }

            return response.Content.ReadAsAsync<Dictionary<string, FileStateResponse>>().Result;
        }

        public byte[] GetFile(string accountName, string workspace, string filePath)
        {
            var requestPath = string.Format("accounts/{0}/workspaces/{1}/files/{2}", accountName, workspace, filePath);
            var response = _httpClient.GetAsync(requestPath).Result;

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsAsync<ErrorResponse>().Result;
                throw new ApiException(string.Format("{0}: {1}", response.StatusCode, error.Message), response);
            }

            return response.Content.ReadAsByteArrayAsync().Result;
        }

        private static ChangeBatchRequest GetPayloadFor(IEnumerable<FinalizedChange> changes, string message = null)
        {
            var payload = new ChangeBatchRequest
            {
                Changes = changes.Select(ChangeRequest.FromChange).ToArray(),
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

            var response = Post(path, payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsAsync<ErrorResponse>().Result;
                throw new ApiException(string.Format("{0}: {1}", response.StatusCode.ToString(), error.Message),
                    response);
            }
        }

        private HttpResponseMessage Post<T>(string path, T payload)
        {
            return _httpClient.PostAsync(path, payload, new JsonMediaTypeFormatter()).Result;
        }
    }
}