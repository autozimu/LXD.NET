using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public struct Image
    {
        public string[] Aliases;
        public string Architecture;
        public bool Cached;
        public string Filename;
        public string Fingerprint;
        public PropertiesStruct Properties;
        public bool Public;
        public int Size;
        public bool AutoUpdate;
        public UpdateSourceStruct UpdateSource;
        public DateTime CreatedAt;
        public DateTime ExpiresAt;
        public DateTime LastUsedAt;
        public DateTime UploadedAt;

        public struct PropertiesStruct
        {
            public string Architecture;
            public string Build;
            public string Description;
            public string Distribution;
            public string Release;
        }

        public struct UpdateSourceStruct
        {
            public string Server;
            public string Protocol;
            public string Certificate;
            public string Alias;
        }
    }
}
