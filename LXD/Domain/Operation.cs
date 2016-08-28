using System.Collections.Generic;

namespace LXD.Domain
{
    public struct Operation
    {
        public string Id;
        public string Class;
        public string CreatedAt;
        public string UpdatedAt;
        public string Status;
        public int StatusCode;
        public Dictionary<string, string[]> Resources;

        // Metadata

        public bool MayCancel;
        public string Err;
    }
}
