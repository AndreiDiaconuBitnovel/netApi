using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using WebApplication2.Models.LogIn;
using WebApplication2.Services.UserService;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Utils;
using System.Net.Mail;
using WebApplication2.DataDB;
using WebApplication2.Services.ImageServiceFolder;
using FaceRecognitionDotNet;


namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration, IImageService imageService)
        {
            _imageService = imageService;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("register")]
        public async Task<AuthResult> Register([FromQuery]string username, [FromQuery]string email)
        {
            UserDetailsRegister userDetails= new UserDetailsRegister()
            {
                Username = username,
                Email = email
            }; 
            IFormFile file = null;
            try
            {
                file = Request.Form.Files.First();
            }
            catch (Exception e)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }

            if (file == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (file.Length <= 0)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg")
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture type, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (userDetails.Email.Equals(String.Empty) || userDetails.Username.Equals(String.Empty))
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid model, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (!IsEmailValid(userDetails.Email))
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid email, please insert a valid email!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (userDetails.Username.Length < 8)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid username, please insert a valid name with length at least 8 characters!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            try
            {
                var emailExistOrUsername = await _userService.GetUsersByEmailOrUsername(userDetails.Email.Trim(), userDetails.Username.Trim());

                if (emailExistOrUsername != null)
                {
                    if (emailExistOrUsername.Count() > 0)
                        return new AuthResult()
                        {
                            Errors = new List<string>() { "Email or username already exists in database" },
                            IsSuccessful = false,
                            Token = ""
                        };
                }

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var fileContent = memoryStream.ToArray();
                var encryptedData = EncryptBytes(fileContent, "SensitivePhrase", "SodiumChloride");

                var userInserted =await  _userService.SaveUserAndImg(userDetails.Email, userDetails.Username, encryptedData);
                if (userInserted == null)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Something is wrong, please try again!" },
                        IsSuccessful = false,
                        Token = ""
                    };
                }
                var token = JwtToken.CreateToken(userInserted.Username, userInserted.Email, userInserted.IdUser.ToString(), _configuration);
                return new AuthResult()
                {
                    Errors = new List<string>(),
                    IsSuccessful = true,
                    Token = token
                };



            }
            catch (Exception e)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { e.Message },
                    IsSuccessful = false,
                    Token = ""
                };
            }

        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("login")]
        public async Task<AuthResult> Login([FromQuery] string username)
        {
            if(username==null || String.Empty.Equals(username))
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid username, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            IFormFile file = null;
            try
            {
                file = Request.Form.Files.First();
            }
            catch (Exception e)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (file == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if (file.Length <= 0)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg")
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture type, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }

            var userDetails = await _userService.GetUserByUsername(username);
            if (userDetails == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid user, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }

            try
            {


                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var fileContent = memoryStream.ToArray();


                var retrievedImage = await _imageService.GetImage(userDetails.IdImg.Value);
                var encryptedPicture = retrievedImage.ImageObj;

                string retrievedImagePathDecrypted = "filesTest\\retrievedImagePathDecrypted.jpg";
                string receivedImagePath = "filesTest\\receivedImagePath.jpg";

                var finalData = DecryptBytes(retrievedImage.ImageObj, "SensitivePhrase", "SodiumChloride");

                System.IO.File.WriteAllBytes(retrievedImagePathDecrypted, finalData);
                System.IO.File.WriteAllBytes(receivedImagePath, fileContent);

                if (!Compare2Images(retrievedImagePathDecrypted, receivedImagePath))
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Not the same face, please try again!" },
                        IsSuccessful = false,
                        Token = ""
                    };
                }

                var token = JwtToken.CreateToken(userDetails.Username, userDetails.Email, userDetails.IdUser.ToString(), _configuration);

                return new AuthResult()
                {
                    Errors = new List<string>(),
                    IsSuccessful = true,
                    Token = token
                };


            }
            catch (Exception ex)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { ex.Message },
                    IsSuccessful = false,
                    Token = ""
                };
            }

        }


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

        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool Compare2Images(string imagePath1, string imagePath2)
        {

            try
            {
                string currentDirectory = "faceModels";
                FaceRecognition fr = FaceRecognition.Create(currentDirectory);

                var dlibToComBuf1 = FaceRecognition.LoadImageFile(imagePath1);
                var enToCompare1 = fr.FaceEncodings(dlibToComBuf1).First();

                var dlibToComBuf2 = FaceRecognition.LoadImageFile(imagePath2);
                var enToCompare2 = fr.FaceEncodings(dlibToComBuf2).First();
                var result = FaceRecognition.CompareFace(enToCompare1, enToCompare2).ToString();

                return result == "True" ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
