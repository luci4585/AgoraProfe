using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services
{
    public class InscripcionService : GenericService<Inscripcion>, IInscripcionService
    {
        public InscripcionService(IMemoryCache? memoryCache = null) : base(memoryCache)
        { 
        }

        public async Task<List<Inscripcion>?> GetInscriptosAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/inscriptos/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<Inscripcion>>(content, _options);
        }

    }
}
