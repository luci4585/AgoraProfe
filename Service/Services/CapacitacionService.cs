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
    public class CapacitacionService : GenericService<Capacitacion>, ICapacitacionService
    {
        public CapacitacionService(IMemoryCache memoryCache) : base(memoryCache)
        {
        }

        public async Task<List<Capacitacion>?> GetCapacitacionesAbiertasAsync()
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/abiertas");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<Capacitacion>>(content, _options);
        }

        public async Task<List<Capacitacion>?> GetCapacitacionesFuturasAsync()
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/futuras");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<Capacitacion>>(content, _options);
        }

    }
}
