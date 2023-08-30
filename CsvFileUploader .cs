using System;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace CsvUploaderApp
{
    public class CsvFileUploader : IFileUploader
    {
        public async Task UploadFilesAsync()
        {
            var accessKey = ConfigurationManager.AppSettings["AccessKey"];
            var secretKey = ConfigurationManager.AppSettings["SecretKey"];
            var bucketName = ConfigurationManager.AppSettings["BucketName"];
            var folderName = ConfigurationManager.AppSettings["FolderName"];
            var srcDirectory = ConfigurationManager.AppSettings["SrcDirectory"];

            var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2);

            try
            {
                var files = Directory.GetFiles(srcDirectory, "*.csv");

                int totalFiles = files.Length;
                int uploadedFiles = 0;
                long totalSize = 0;

                var stopwatch = Stopwatch.StartNew();

                foreach (var file in files)
                {
                    using (var fileTransferUtility = new TransferUtility(s3Client))
                    {
                        var fileName = Path.GetFileName(file);
                        var s3Key = $"{folderName}/{fileName}";

                        await fileTransferUtility.UploadAsync(file, bucketName, s3Key);

                        Console.WriteLine($"Uploaded {fileName} to S3.");
                        uploadedFiles++;
                        totalSize += new FileInfo(file).Length;
                    }
                }

                stopwatch.Stop();
                TimeSpan elapsedTime = stopwatch.Elapsed;

                Console.WriteLine($"Number of CSV files: {totalFiles}");
                Console.WriteLine($"Actual uploaded files: {uploadedFiles}");
                Console.WriteLine($"Total data uploaded: {totalSize / 1024} KB");
                Console.WriteLine($"Total time taken: {elapsedTime.TotalSeconds} seconds");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Amazon S3 Exception: {ex.Message}");
                // Perform additional exception handling here
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                // Perform additional exception handling here
            }
        }
    }
}
