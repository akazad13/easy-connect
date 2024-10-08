using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;

namespace Adda.API.ExternalServices.Cloudinary;

public class CloudinaryService(
    ICloudinary cloudinary
    ) : ICloudinaryService
{
    private readonly ICloudinary _cloudinary = cloudinary;

    public async Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(IFormFile file)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                return uploadResult.Error switch
                {
                    null => new PhotoUploadResult(uploadResult.Url.ToString(), uploadResult.PublicId),
                    _ => ErrorOr.Error.Failure(description: uploadResult.Error.Message),
                };
            }
            return ErrorOr.Error.Failure(description: "File not found!");
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeletePhotoAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result switch
            {
                "ok" => Result.Success,
                _ => ErrorOr.Error.Failure(description: result.Error.Message),
            };
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }
}

public record PhotoUploadResult(string Url, string PublicId);
