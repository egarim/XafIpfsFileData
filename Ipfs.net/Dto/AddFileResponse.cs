using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ipfs.net
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string Type { get; set; }
    }

    public class AddFileResponse
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Size { get; set; }
    }





}
