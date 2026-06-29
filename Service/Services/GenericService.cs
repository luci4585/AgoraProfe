using DotNetEnv;
using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        protected HttpClient _httpClient;
        protected JsonSerializerOptions _options;
        protected string _endpoint;
        protected IMemoryCache? _memoryCache;


        public GenericService(IMemoryCache memoryCache) {
            _memoryCache = memoryCache;
            SettingHttpClient();
            _endpoint = ApiEndpoints.GetEndpoint(typeof(T).Name);

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

        protected void SetAuthorizationHeader()
        {
            // Si ya está configurado (por un DelegatingHandler), no hacer nada
            if (_httpClient.DefaultRequestHeaders.Authorization is not null)
                return;

            // 1) Intentar leer desde IMemoryCache (configurado por FirebaseAuthService)
            if (_memoryCache is not null && _memoryCache.TryGetValue("jwt", out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedToken);
                return;
            }


            // Si no se definió el token, se lanza una excepción

            throw new InvalidOperationException("El token JWT no está disponible para la autorización.");

        }

        public async Task<T?> AddAsync(T? entity)
        {
            var response = await _httpClient.PostAsJsonAsync(_endpoint, entity);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al agregar el dato: {response.StatusCode} - {content}");
            }
            return JsonSerializer.Deserialize<T>(content, _options);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al eliminar el dato: {response.StatusCode}");
            }
            return response.IsSuccessStatusCode;

        }

        public async Task<List<T>?> GetAllAsync(string? filtro="")
        {
            var response= await _httpClient.GetAsync($"{_endpoint}?filter={filtro}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<T>>(content, _options);

        }

        public async Task<List<T>?> GetAllDeletedsAsync(string? filtro="")
        {
            var response = await _httpClient.GetAsync($"{_endpoint}/deleteds");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<T>>(content, _options);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_endpoint}/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<T>(content, _options);
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var response = await _httpClient.PutAsync($"{_endpoint}/restore/{id}",null);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al restaurar el dato: {response.StatusCode}");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(T? entity)
        {
            var idValue=entity.GetType().GetProperty("Id").GetValue(entity);
            var response= await _httpClient.PutAsJsonAsync($"{_endpoint}/{idValue}", entity);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Hubo un problema al actualizar{ response.StatusCode } - { content}");
            } else
            {
                return response.IsSuccessStatusCode;
            }

        }
    }
}
