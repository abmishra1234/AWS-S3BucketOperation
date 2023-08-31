using CsvUploaderApp;
using S3BucketOperation;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // for Uploading the CSV File
        IFileUploader fileUploader = new CsvFileUploader();
        await fileUploader.UploadFilesAsync();

        // For Downloading the CSV File
        IFileDownloader fileDownLoader = new CsvFileDownloader();
        fileDownLoader.DownloadCsvFiles();
    }
}
