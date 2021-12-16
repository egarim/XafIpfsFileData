using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.net.Dto
{
    public class Arguments
    {
        public string DirHash { get; set; }
    }

    public class Link
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
    }

    public class Directory
    {
        public string Hash { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Objects
    {
        public Directory Directory { get; set; }
    }

    public class LsResponse
    {
        public Arguments Arguments { get; set; }
        public Objects Objects { get; set; }
    }
   
}
