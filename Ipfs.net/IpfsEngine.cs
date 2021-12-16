using Ipfs.net.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//hack https://docs.ipfs.io/reference/http/api/#api-v0-add


namespace Ipfs.net
{
    public class IpfsEngine
    {
        public string ApiUrl { get; set; }
        HttpClient httpClient = new HttpClient();
        public IpfsEngine(string apiUrl)
        {
            ApiUrl = apiUrl;
        }
       
        public async Task<IEnumerable<AddFileResponse>> Add(byte[] content, string fileName, IDictionary<string, string> Options = null)
        {
            List<AddFileResponse> AddFileResponses = new List<AddFileResponse>();
            // In production code, don't destroy the HttpClient through using, but better reuse an existing instance
            // https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{ApiUrl}/api/v0/add"))
            {
                var multipartContent = new MultipartFormDataContent();
                ByteArrayContent content1 = new ByteArrayContent(content);
                multipartContent.Add(content1, "file", fileName);
                request.Content = multipartContent;

                var response = await httpClient.SendAsync(request);
                string value = await response.Content.ReadAsStringAsync();
                var result = value.Split(new[] { '\r', '\n' });
                foreach (var item in result)
                {
                    if(item!="")
                    {
                        AddFileResponses.Add(JsonConvert.DeserializeObject<AddFileResponse>(item));
                    }
                }
             
                return AddFileResponses;
            }
          
        }
        public async Task<IEnumerable<AddFileResponse>> Add(string FilePath, string fileName, IDictionary<string, string> Options = null)
        {
            return await Add(File.ReadAllBytes(FilePath), fileName);
        }
        public async Task<byte[]> Get(string filePath, IDictionary<string, string> Options = null)
        {
            Options = PrepareFileArgs(filePath, Options,false);
            var Url = this.BuildUrl(ApiUrl + "/api/v0/get", Options);
            byte[] bytes;
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), Url))
            {
                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string value = await response.Content.ReadAsStringAsync();

                    throw new Exception(value);
                }
                bytes = await response.Content.ReadAsByteArrayAsync();
            }
            return bytes;
        }
        public async Task<string> Cat(string path, IDictionary<string, string> Options = null)
        {
            Options = PrepareFileArgs(path, Options, false);
            var Url = this.BuildUrl(ApiUrl + "/api/v0/cat", Options);
            string value = string.Empty;
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), Url))
            {
                var response = await httpClient.SendAsync(request);
                value = await response.Content.ReadAsStringAsync();
            }
            return value;
        }


        #region Files

        public async Task<LsResponse> FilesLs(string path, IDictionary<string, string> Options = null)
        {
            Options = PrepareFileArgs(path, Options, false);
            var Url = this.BuildUrl(ApiUrl + "/api/v0/file/ls", Options);
            string value = string.Empty;
            LsResponse dir;
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), Url))
            {
                var response = await httpClient.SendAsync(request);
                value = await response.Content.ReadAsStringAsync();
                value= value.ReplaceNthOccurrence(path, "DirHash", 1).ReplaceNthOccurrence(path, "Directory", 2);
                dir = JsonConvert.DeserializeObject<LsResponse>(value);
            }
            return dir;
        }

        public async Task FilesRm(string hash, IDictionary<string, string> Options = null)
        {
            Options = PrepareFileArgs(hash, Options, true);
            var Url = this.BuildUrl(ApiUrl + "/api/v0/files/rm", Options);

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), Url))
            {
                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string value = await response.Content.ReadAsStringAsync();

                    throw new Exception(value);
                }


            }

        }
        public async Task FilesMkdir(string hash, IDictionary<string, string> Options = null)
        {
            Options = PrepareFileArgs(hash, Options, true);
            var Url = this.BuildUrl(ApiUrl + "/api/v0/files/mkdir", Options);

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), Url))
            {
                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string value = await response.Content.ReadAsStringAsync();

                    throw new Exception(value);
                }


            }

        }
        #endregion

        #region Internal

        protected virtual string BuildUrl(string Url, IDictionary<string, string> Options)
        {
            if (Options != null)
            {
                Url = Url + "?";
            }
            foreach (KeyValuePair<string, string> option in Options)
            {
                Url = Url + option.Key + "=" + option.Value;
            }
            return Url;

        }
        private static IDictionary<string, string> PrepareFileArgs(string hash, IDictionary<string, string> Options, bool StartPathWithSlash)
        {
            if (Options == null)
            {
                Options = new Dictionary<string, string>();
            }
            if (Options.ContainsKey("arg"))
            {
                Options["arg"] = hash;
            }
            else
            {
                if (StartPathWithSlash)
                    Options.Add("arg", "/" + hash);
                else
                    Options.Add("arg", "" + hash);
            }

            return Options;
        }
       

        #endregion

        
    }
    public static class StringExtensionMethods
    {
       
        public static string ReplaceNthOccurrence(this string obj, string find, string replace, int nthOccurrence)
        {
            if (nthOccurrence > 0)
            {
                MatchCollection matchCollection = Regex.Matches(obj, Regex.Escape(find));
                if (matchCollection.Count >= nthOccurrence)
                {
                    Match match = matchCollection[nthOccurrence - 1];
                    return obj.Remove(match.Index, match.Length).Insert(match.Index, replace);
                }
            }
            return obj;
        }
    }
}
