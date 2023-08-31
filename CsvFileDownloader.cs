using System;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Configuration;

namespace S3BucketOperation
{
    public class CsvFileDownloader : IFileDownloader
    {
        private int totalFilesToDownload = 0;
        private int filesDownloaded = 0;
        private DateTime startTime;

        public void DownloadCsvFiles()
        {
            var accessKey = ConfigurationManager.AppSettings["AccessKey"];
            var secretKey = ConfigurationManager.AppSettings["SecretKey"];
            var bucketName = ConfigurationManager.AppSettings["BucketName"];
            var sourceFolder = ConfigurationManager.AppSettings["FolderName"];
            var destinationFolder = ConfigurationManager.AppSettings["destDirectory"];

            try
            {
                startTime = DateTime.Now;
                using (var s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.USWest2))
                {
                    ListObjectsV2Request request = new ListObjectsV2Request
                    {
                        BucketName = bucketName,
                        Prefix = sourceFolder
                    };

                    ListObjectsV2Response response;
                    do
                    {
                        response = s3Client.ListObjectsV2(request);

                        foreach (var obj in response.S3Objects)
                        {
                            if (obj.Key.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                            {
                                DownloadFile(bucketName, s3Client, obj.Key, destinationFolder);
                            }
                        }

                        request.ContinuationToken = response.NextContinuationToken;
                    } while (response.IsTruncated);
                }

                // Calculate and print statistics
                TimeSpan totalTime = DateTime.Now - startTime;
                Console.WriteLine($"Total files to download: {totalFilesToDownload}");
                Console.WriteLine($"Files downloaded: {filesDownloaded}");
                Console.WriteLine($"Total time taken: {totalTime.Seconds} Seconds");
            }
            catch (Exception ex)
            {
                // Handle exceptions and log the error
                Console.WriteLine("An error occurred: " + ex.Message);
                // Log the error using your preferred logging mechanism (e.g., log4net, NLog)
                // Example: Logger.Log(ex);
            }
        }

        private void DownloadFile(string bucketName, AmazonS3Client s3Client, string objectKey, 
            string filePath)
        {
            // Increment the files downloaded counter
            filesDownloaded++;

            // Below code is for extracting the fileName from object file
            string fileName = Path.GetFileName(objectKey);
            filePath = Path.Combine(filePath, fileName);

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            using (GetObjectResponse response = s3Client.GetObject(request))
            using (Stream responseStream = response.ResponseStream)
            using (FileStream fileStream = File.Create(filePath))
            {
                responseStream.CopyTo(fileStream);
            }
        }
        // Rest of the code is the same as before
    }
}
