using System;

namespace LXD.Domain
{
    public class Image : RemoteObject
    {
        public string[] Aliases;
        public string Architecture;
        public bool Cached;
        public string Filename;
        public string Fingerprint;
        public ImageProperties Properties;
        public bool Public;
        public int Size;
        public bool AutoUpdate;
        public ImageUpdateSource UpdateSource;
        public DateTime CreatedAt;
        public DateTime ExpiresAt;
        public DateTime LastUsedAt;
        public DateTime UploadedAt;

        public struct ImageProperties
        {
            public string Architecture;
            public string Build;
            public string Description;
            public string Distribution;
            public string Release;
        }

        public struct ImageUpdateSource
        {
            public string Server;
            public string Protocol;
            public string Certificate;
            public string Alias;
        }
    }
}
