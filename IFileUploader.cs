using System.Threading.Tasks;

namespace CsvUploaderApp
{
    public interface IFileUploader
    {
        Task UploadFilesAsync();
    }
}
