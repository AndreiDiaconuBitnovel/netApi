using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;
using System.Drawing.Imaging;
using FaceRecognitionDotNet;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.DataDB;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageProcessing : ControllerBase
    {
        private readonly TeamDropDatabaseContext _teamDropDatabaseContext;
        private readonly IConfiguration _configuration;
        public ImageProcessing(TeamDropDatabaseContext teamDropDatabaseContext, IConfiguration configuration)
        {
            _teamDropDatabaseContext = teamDropDatabaseContext;
            _configuration = configuration;
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("compareTwoImg")]
        public async Task<IActionResult> CompareTwoImg()
        {
            string imagePath1 = "filesTest\\firstImg.jpg";
            string imagePath2 = "filesTest\\firstImg.jpg";

            try
            {
                string currentDirectory = "faceModels";
                FaceRecognition fr = FaceRecognition.Create(currentDirectory);

                var dlibToComBuf1 = FaceRecognition.LoadImageFile(imagePath1);
                var enToCompare1 = fr.FaceEncodings(dlibToComBuf1).First();

                var dlibToComBuf2 = FaceRecognition.LoadImageFile(imagePath2);
                var enToCompare2 = fr.FaceEncodings(dlibToComBuf2).First();
                var result = FaceRecognition.CompareFace(enToCompare1, enToCompare2).ToString();

                return result == "True" ? Ok(true) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("saveImgInDb")]
        public async Task<IActionResult> SaveImgInDb()
        {

            string imagePath = "filesTest\\Alex.jpg";
            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);

            var encryptedData = EncryptBytes(imageData, "SensitivePhrase", "SodiumChloride");

            var imageEntity = new DataDB.Image { Id = Guid.NewGuid(), ImageObj = encryptedData };

            _teamDropDatabaseContext.Images.Add(imageEntity);
            _teamDropDatabaseContext.SaveChanges();


            var retrievedImage = await _teamDropDatabaseContext.Images.FirstOrDefaultAsync(X => X.Id == imageEntity.Id);
            var encryptedPicture = retrievedImage.ImageObj;

            string retrievedImagePathDecrypted = "filesTest\\retrievedImagePathDecrypted.jpg";
            string retrievedImagePathEncrypt = "filesTest\\retrievedImagePathEncrypt.jpg";

            var finalData = DecryptBytes(retrievedImage.ImageObj, "SensitivePhrase", "SodiumChloride");

            System.IO.File.WriteAllBytes(retrievedImagePathDecrypted, finalData);
            System.IO.File.WriteAllBytes(retrievedImagePathEncrypt, encryptedPicture);


            return Ok(true);
        }

        // Example usage: EncryptBytes(someFileBytes, "SensitivePhrase", "SodiumChloride");
        public static byte[] EncryptBytes(byte[] inputBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return CipherBytes;
        }

        // Example usage: DecryptBytes(encryptedBytes, "SensitivePhrase", "SodiumChloride");
        public static byte[] DecryptBytes(byte[] encryptedBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(encryptedBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] plainBytes = new byte[encryptedBytes.Length];

            int DecryptedCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return plainBytes;
        }

        public static void RemoveImageFromPath(string filePath)
        {
            System.IO.File.Delete(filePath);
        }
    }
}

