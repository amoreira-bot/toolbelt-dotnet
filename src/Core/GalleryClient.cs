using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Vtex.Toolbelt.Core
{
    public class GalleryClient
    {
        private readonly string accountName;
        private readonly string sessionName;
        private readonly string rootPath;
        private readonly HttpClient httpClient;

        public GalleryClient(string accountName, string sessionName, string rootPath)
        {
            this.accountName = accountName;
            this.sessionName = sessionName;
            this.rootPath = rootPath;
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://gallery.vtexlocal.com.br/api/gallery/")
            };
        }

        public event Action<RequestFailedArgs> RequestFailed;

        public IEnumerable<Change> SendChanges(Change[] changes, bool resync = false)
        {
            var result = SummarizeChanges(changes).ToArray();
            this.SendRequest(result, resync);
            return result;
        }

        protected IEnumerable<Change> SummarizeChanges(IEnumerable<Change> changes)
        {
            changes = changes.Reverse();
            var deletedPaths = new List<string>();
            var updatedPaths = new List<string>();
            foreach (var change in changes)
            {
                switch (change.Action)
                {
                    case ChangeAction.Update:
                        if (ShouldUpdate(change.Path, updatedPaths, deletedPaths))
                            updatedPaths.Add(change.Path);
                        break;

                    case ChangeAction.Delete:
                        if (ShouldDelete(change.Path, updatedPaths, deletedPaths))
                            deletedPaths.Add(change.Path);
                        break;
                }
            }
            return updatedPaths.Select(path => new Change(ChangeAction.Update, NormalizePath(path)))
                .Union(deletedPaths.Select(path => new Change(ChangeAction.Delete, NormalizePath(path))));
        }

        private static bool ShouldUpdate(string changePath, ICollection<string> updatedPaths,
            ICollection<string> deletedPaths)
        {
            return !deletedPaths.Contains(changePath)
                   && !updatedPaths.Contains(changePath)
                   && !deletedPaths.Any(deleted => changePath.StartsWith(deleted + Path.DirectorySeparatorChar));
        }

        private static bool ShouldDelete(string changePath, ICollection<string> updatedPaths,
            ICollection<string> deletedPaths)
        {
            return !updatedPaths.Contains(changePath)
                   && !deletedPaths.Contains(changePath);
        }

        protected virtual string NormalizePath(string path)
        {
            return path.Substring(this.rootPath.Length + 1).Replace('\\', '/');
        }

        private void SendRequest(IEnumerable<Change> result, bool resync = false)
        {
            var payload = new ChangeBatchRequest
            {
                AccountName = this.accountName,
                Session = this.sessionName,
                UserCookie = "fakevalue",
                Changes = result.Select(change => ChangeRequest.FromChange(change, this.rootPath)).ToArray()
            };

            var response = this.httpClient.PostAsync("development/changes" + (resync ? "?resync=true" : ""),
                payload, new JsonMediaTypeFormatter()).Result;

            if (!response.IsSuccessStatusCode)
            {
                var args = new RequestFailedArgs(response.StatusCode.ToString(), (int) response.StatusCode);
                this.RequestFailed(args);
            }
        }
    }

    public class RequestFailedArgs
    {
        public string StatusCodeText { get; private set; }
        public int StatusCode { get; private set; }

        public RequestFailedArgs(string statusCodeText, int statusCode)
        {
            StatusCodeText = statusCodeText;
            StatusCode = statusCode;
        }
    }
}
