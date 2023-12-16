using WebApplication2.DataDB;

namespace WebApplication2.Services.ImageServiceFolder
{
    public interface IImageService
    {
        Task<Guid> InsertImage(byte[] fileContent);
        Task<Image> GetImage(Guid id);
    }
}
