using DotNetEnv;
using Microsoft.Extensions.Caching.Memory;
using Service.DTOs;
using Service.Interfaces;
using Service.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        HttpClient _httpClient;
        JsonSerializerOptions _options;
        IMemoryCache _memoryCache;
        public AuthService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            SettingHttpClient();
        }

        private void SettingHttpClient()
        {
            Env.Load();
            var apiUrl = Environment.GetEnvironmentVariable("API_URL");
            if (apiUrl == null)
            {
                throw new InvalidOperationException("La variable de entorno 'API_URL' no está definida.");
            }
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(apiUrl);
            _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password, string nombre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nombre))
            {
                throw new ArgumentException("Email, password o nombre no pueden ser nulos o vacíos.");
            }
            try
            {
                var UrlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var newUser = new RegisterDTO { Email = email, Password = password, Nombre = nombre };
                var response = await client.PostAsJsonAsync($"{UrlApi}{endpointAuth}/register/", newUser);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    GenericService<object>.jwtToken = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear usuario" + ex.Message);
            }

        }

        //si no recibo el objeto IConfiguration en el constructor, creo un constructor vacio que instancie uno y lea el archivo appsettings.json


        public async Task<string?> Login(LoginDTO? login)
        {
            if (login == null)
            {
                throw new ArgumentException("El objeto login no llego.");
            }
            try
            {
                var urlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{urlApi}{endpointAuth}/login/", login);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    GenericService<object>.jwtToken = result;
                    return null;
                }
                else
                {
                    //si no es exitoso, devuelvo el mensaje de error
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return errorContent;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al loguearse->: " + ex.Message);
            }




        }
        public async Task<bool> ResetPassword(LoginDTO? login)
        {
            if (login == null)
            {
                throw new ArgumentException("El objeto login no llego.");
            }
            try
            {
                var urlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{urlApi}{endpointAuth}/resetpassword/", login);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al resetear el password->: " + ex.Message);
            }
        }
        public async Task<bool> DeleteUser(LoginDTO? login)
        {
            if (login == null)
            {
                throw new ArgumentException("El objeto login no llego.");
            }
            try
            {
                var urlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{urlApi}{endpointAuth}/deleteuser/", login);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario en firebase->: " + ex.Message);
            }
        }
    }
}

