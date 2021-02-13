using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadPdfApi.Model
{
    public class BlobFileInfo
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public long? FileSize { get; set; }
    }
}
