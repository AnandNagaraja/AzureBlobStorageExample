using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadPdfApi.Model
{
    /// <summary>
    /// Uploaded files from blob storage with Name, Location and FileSize
    /// </summary>
    public class BlobFileInfo
    {
        /// <summary>
        /// Name of the uploaded file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the uploaded file
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// FileSize of the uploaded File
        /// </summary>
        public long? FileSize { get; set; }
    }
}
