using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

namespace FileUploader.Services
{
    public interface IFileUpload
    {
        Task UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUrl(IBrowserFile file);
    }
    public class FileUpload : IFileUpload
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileUpload> _logger;

        public FileUpload(IWebHostEnvironment webHostEnvironment, ILogger<FileUpload> logger)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task UploadFile(IBrowserFile file)
        {
            if(file is not null)
            {
                try
                {
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"uploads", file.Name);

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
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.Message);
                }
            }
        }

        public async Task<string> GeneratePreviewUrl(IBrowserFile file)
        {
            if (!file.ContentType.Contains("image"))
            {
                if (file.ContentType.Contains("pdf"))
                {
                    return "images/PDF_icon.png";
                }
            }

            var resizedImage = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        }
    }
}
