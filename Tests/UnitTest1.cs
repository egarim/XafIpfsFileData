using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;
using Ipfs.net;
using Newtonsoft.Json;
using Ipfs.net.Dto;
namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");
            var result = await ipfsEngine.Add("st-petersburg-resize.jpg", "st-petersburg-resize.jpg");
            Debug.WriteLine(result.FirstOrDefault().Hash);
            var FileData=   await ipfsEngine.Get(result.FirstOrDefault().Hash);
            await  File.WriteAllBytesAsync("newfile.jpg", FileData);
            await  ipfsEngine.FilesRm(result.FirstOrDefault().Hash);
         

            Assert.Pass();
        }
        [Test]
        public async Task CatTest()
        {
            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");
            var result = await ipfsEngine.Add("TextFile.txt", "TextFile.txt");
            Debug.WriteLine(result.FirstOrDefault().Hash);
            var FileData = await ipfsEngine.Get(result.FirstOrDefault().Hash);
            //await File.WriteAllBytesAsync("newfile.jpg", FileData);
            var content=   await ipfsEngine.Cat(result.FirstOrDefault().Hash);
            await ipfsEngine.FilesRm(result.FirstOrDefault().Hash);


            Assert.Pass();
        }
        [Test]
        public async Task FilesMkdirTest()
        {
            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");

            const string FolderName = "MyDirectory-9c6c551c-59a5-422e-830b-1d6ac2174819";
            //await ipfsEngine.FilesMkdir(FolderName);
            var result = await ipfsEngine.Add("TextFile.txt", $"{FolderName}/TextFile21.txt");
            //var content = await ipfsEngine.Cat(result.Hash);
            await ipfsEngine.FilesRm(result.FirstOrDefault().Hash);
            Assert.Pass();
        }
        [Test]
        public async Task FIlesLsTest()
        {
            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");

            string FolderName = $"MyDirectory-{Guid.NewGuid().ToString()}";
            //await ipfsEngine.FilesMkdir(FolderName);
            var result = await ipfsEngine.Add("TextFile.txt", $"{FolderName}/TextFile21.txt");
            var content = await ipfsEngine.FilesLs(result.LastOrDefault().Hash);
            
            Assert.Pass();
        }
        [Test]
        public void DeserializeLsResponse()
        {
            string LsResponse = File.ReadAllText("LsResponse.json");
           
            const string ObjectName = "QmQzDkUtFPRH7es2hUM5KPWVm3sHd1N8KMeUbX6xvBaXjM";
            const string Directory = "Directory";

            LsResponse = LsResponse.ReplaceNthOccurrence(ObjectName, "DirHash", 1).ReplaceNthOccurrence(ObjectName, "Directory", 2);

            var LsResponseObject=JsonConvert.DeserializeObject<LsResponse>(LsResponse);
            Assert.Pass();
        }
    }
}