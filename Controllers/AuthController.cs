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


namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("register")]
        public async Task<AuthResult> Register(UserDetailsRegister userDetails)
        {
            var file = Request.Form.Files.First();
            
            if(file == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid picture, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            if(file.Length <= 0)
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
            if(userDetails.Email.Equals(String.Empty) || userDetails.Username.Equals(String.Empty))
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid model, please try again!" },
                    IsSuccessful = false,
                    Token = ""
                };
            }
            try
            {
                var emailExistOrUsername = await _userService.GetUsersByEmailOrUsername(userDetails.Email.Trim().ToLower(), userDetails.Username.Trim().ToLower());

                if (emailExistOrUsername != null)
                {
                    if (emailExistOrUsername.Count() > 0)
                        return new AuthResult()
                        {
                            Errors = new List<string>() { "Email already exists in database" },
                            IsSuccessful = false,
                            Token = ""
                        };
                }

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var fileContent = memoryStream.ToArray();
                var encryptedData = EncryptBytes(fileContent, "SensitivePhrase", "SodiumChloride");

                var userInserted = _userService.SaveUserAndImg(userDetails.Email, userDetails.Username, encryptedData);
                if (userInserted == null)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() {"Something is wrong, please try again!"},
                        IsSuccessful = false,
                        Token = ""
                    };
                }
                var token = JwtToken.CreateToken(userDetails.Username, _configuration);
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
        public async Task<AuthResult> Login([FromBody] String username)
        {
            return null;
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
    }
}
