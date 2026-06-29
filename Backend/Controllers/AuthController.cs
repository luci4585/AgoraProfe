using DotNetEnv;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        FirebaseAuthClient firebaseAuthClient;
        FirebaseAuthConfig _config;

        public AuthController()
        {
            SetFirebaseConfig();
        }

        private void SetFirebaseConfig()
        {
            Env.Load();
            var apikey = Environment.GetEnvironmentVariable("FIREBASE_APIKEY");
            var authdomain = Environment.GetEnvironmentVariable("FIREBASE_AUTHDOMAIN");

            _config = new FirebaseAuthConfig
            {
                ApiKey = apikey,
                AuthDomain = authdomain,
                Providers = new FirebaseAuthProvider[]
                   {
                    new EmailProvider()
                   },
            };

            firebaseAuthClient = new FirebaseAuthClient(_config);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                var credentials = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(login.Username, login.Password);

                if (credentials.User.Info.IsEmailVerified == false)
                {
                    return BadRequest("Email no verificado. Verifica tu correo antes de iniciar sesión.");
                }
                return Ok(credentials.User.GetIdTokenAsync().Result);
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.Reason.ToString());
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                var user = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(register.Email, register.Password, register.Nombre);
                await SendVerificationEmailAsync(user.User.GetIdTokenAsync().Result);
                return Ok(user.User.GetIdTokenAsync().Result);
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception(ex.Reason.ToString());
            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] LoginDTO login)
        {
            try
            {
                await firebaseAuthClient.ResetEmailPasswordAsync(login.Username);
                return Ok();
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = ex.Reason.ToString() });
            }
        }

        [HttpPost("deleteuser")]
        public async Task<IActionResult> DeleteUser([FromBody] LoginDTO login)
        {
            try
            {
                var credentials = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(login.Username, login.Password);
                await firebaseAuthClient.User.DeleteAsync();
                return Ok();
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = ex.Reason.ToString() });
            }
        }

        [HttpPost("sendverificationemail")]


        public async Task SendVerificationEmailAsync([FromBody]string idToken)
        {
            var RequestUri = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + _config.ApiKey;
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent("{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + idToken + "\"}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(RequestUri, content);
                response.EnsureSuccessStatusCode();
            }
        }


    }
}
