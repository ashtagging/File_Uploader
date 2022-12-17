using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

namespace FileUploader.Services
{
    public interface IFileUpload
    {
        Task UploadFile(IBrowserFile file);
    }
    public class FileUpload : IFileUpload
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileUpload> _logger;

        public FileUpload(IWebHostEnvironment webHostEnvironment, ILogger<FileUpload> logger)
        {
            logger = _logger;
            webHostEnvironment = _webHostEnvironment;
        }
        public async Task UploadFile(IBrowserFile file)
        {
            if(file != null)
            {
                try
                {
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.Name);

                    using(var stream = file.OpenReadStream())
                    {
                        var fileStream = File.Create(uploadPath);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
                catch(Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }
    }
}
