﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3BucketOperation
{
    public interface IFileDownloader
    {
        void DownloadCsvFiles();
    }
}
