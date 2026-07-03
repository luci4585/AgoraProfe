using Microsoft.Extensions.Caching.Memory;
using Service.DTOs;
using Service.Interfaces;
using Service.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Services
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        public UsuarioService(IMemoryCache? memoryCache = null) : base(memoryCache)
        {

        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/byemail?email={email}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<Usuario>(content, _options);
        }
        public async Task<bool> LoginInSystem(string email, string password)
        {
            var loginDTO = new LoginDTO
            {
                Username = email,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync($"Auth/login", loginDTO);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al iniciar sesión: {response.StatusCode}");
            }
            // El endpoint retorna el JWT como string JSON ("token") o texto plano (token).
            // Se eliminan las comillas del encoding JSON si están presentes.
            var token = content.Trim('"');
            _memoryCache!.Set("jwt", token);
            return response.IsSuccessStatusCode;
        }

    }
}
