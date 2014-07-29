using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class GalleryClient
    {
        private readonly string accountName;
        private readonly string workspaceName;
        private readonly string rootPath;
        private readonly HttpClient httpClient;

        public GalleryClient(string accountName, string workspaceName, string rootPath, string authenticationToken,
            string endpointUrl)
        {
            this.accountName = accountName;
            this.workspaceName = workspaceName;
            this.rootPath = rootPath;
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(endpointUrl)
            };
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token",
                authenticationToken);
        }

        public void SendChanges(IEnumerable<Change> result, bool resync = false)
        {
            var payload = new ChangeBatchRequest
            {
                Message = "File changes via VTEX Toolbelt",
                Changes = result.Select(change => ChangeRequest.FromChange(change, this.rootPath)).ToArray()
            };

            var path = string.Format("accounts/{0}/{1}/changes{2}", this.accountName, this.workspaceName,
                resync ? "?resync=true" : "");

            var response = this.httpClient.PostAsync(path, payload, new JsonMediaTypeFormatter()).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(string.Format("Failed to send changes with status code {0} ({1})",
                    (int) response.StatusCode, response.StatusCode), response);
            }
        }
    }
}
