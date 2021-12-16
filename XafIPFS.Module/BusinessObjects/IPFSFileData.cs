using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace XafIPFS.Module.BusinessObjects
{
    [DefaultClassOptions]
	[DefaultProperty(nameof(FileName))]
	public class IPFSFileData : BaseObject, IFileData, IEmptyCheckable
	{
        string ipfsUrl;
        string content;
        private string fileName = "";
#if MediumTrust
		private int size;
		public int Size {
			get { return size; }
			set { SetPropertyValue("Size", ref size, value); }
		}
#else
        [Persistent]
        private int size;
        public int Size
        {
            get { return size; }
        }
#endif
        public IPFSFileData(Session session) : base(session) { }
        public void LoadFromStream(string fileName, Stream stream)
        {

            var task = System.Threading.Tasks.Task.Run(async () => await SaveToIPFS(fileName, stream));
            

            //byte[] bytes = new byte[stream.Length];
            //stream.Read(bytes, 0, bytes.Length);
            //Content = bytes;
        }

        private async System.Threading.Tasks.Task SaveToIPFS(string fileName, Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");
            FileName = fileName;

            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);


            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");
            var result = await ipfsEngine.Add(ms.ToArray(), fileName);
            Debug.WriteLine(result.FirstOrDefault().Hash);

            //https://ipfs.io/ipfs/QmQPEi2mA9VyUowLwQvabt7rJU1xcqtn29KSuQRHL2dsCT
            Content = result.FirstOrDefault().Hash;
            this.IpfsUrl = $"https://ipfs.io/ipfs/{Content}";
            //this.Content=
        }

        public virtual void SaveToStream(Stream stream)
        {


            System.Threading.Tasks.Task<byte[]> task=null;
            if (Content != null)
            {
                task = System.Threading.Tasks.Task<byte[]>.Run(async () => await DownloadFile(stream));
                task.Wait();
            }

            MemoryStream memoryStream = new MemoryStream(task.Result);
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(stream);
            stream.Flush();
        }

        private async System.Threading.Tasks.Task<byte[]> DownloadFile(Stream stream)
        {
            Ipfs.net.IpfsEngine ipfsEngine = new Ipfs.net.IpfsEngine("http://127.0.0.1:5001");

            byte[] vs = await ipfsEngine.Get(this.Content);
       
            return vs;
           
        }

        public void Clear()
        {
            Content = null;
            FileName = String.Empty;
        }
        public override string ToString()
        {
            return FileName;
        }
        [Size(260)]
        public string FileName
        {
            get { return fileName; }
            set { SetPropertyValue(nameof(FileName), ref fileName, value); }
        }


        
        [Size(SizeAttribute.Unlimited)]
        public string IpfsUrl
        {
            get => ipfsUrl;
            set => SetPropertyValue(nameof(IpfsUrl), ref ipfsUrl, value);
        }
        [MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Content 
        {
            get => content;
            set => SetPropertyValue(nameof(Content), ref content, value);
        }
        #region IEmptyCheckable Members
        [NonPersistent, MemberDesignTimeVisibility(false)]
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(FileName); }
		}
		#endregion
	}
}