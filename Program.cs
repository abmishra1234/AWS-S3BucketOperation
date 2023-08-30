using CsvUploaderApp;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        IFileUploader fileUploader = new CsvFileUploader();
        await fileUploader.UploadFilesAsync();
    }
}
