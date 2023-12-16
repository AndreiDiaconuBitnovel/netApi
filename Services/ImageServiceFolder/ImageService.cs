using WebApplication2.DataDB;

namespace WebApplication2.Services.ImageServiceFolder
{
    public class ImageService : IImageService
    {
        private readonly TeamDropDatabaseContext _testContext;

        public ImageService(TeamDropDatabaseContext testContext)
        {
            _testContext = testContext;
        }
        public async Task<Guid> InsertImage(byte[] fileContent)
        {
            try { 
            var image = new Image()
            {Id=Guid.NewGuid(),
            ImageObj=fileContent
            };
            await _testContext.Images.AddAsync(image);
            await _testContext.SaveChangesAsync();
                return image.Id;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }
    }
}
