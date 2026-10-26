using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace yakovleva_pr7_v25
{
    public class EquipmentApi
    {
        private readonly HttpClient _client;
        public EquipmentApi(HttpClient client)
        {
            _client = client;
        }
        public async Task<List<Equipment>> GetEquipments(bool? operational = null, decimal? minPrice = null)
        {
            try
            {
                var url = "api/v1/Equipment";
                var query = new List<string>();

                if (operational != null)
                    query.Add($"operational={operational.Value.ToString().ToLower()}");

                if (minPrice != null)
                    query.Add($"minPrice={minPrice.Value}");

                if (query.Count > 0)
                    url += "?" + string.Join("&", query);

                return await _client.GetFromJsonAsync<List<Equipment>>(url);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
        public async Task<string> CreateEquipment(Equipment equipment)
        {
            try
            {
                var response = await _client.PostAsJsonAsync<Equipment>("api/v1/Equipment", equipment);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
        }
        public async Task<string> PatchEquipment(int id, object updates)
        {
            try
            {
                var response = await _client.PatchAsJsonAsync($"api/v1/Equipment/{id}", updates);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
}
        public async Task<string> PutEquipment(int id, Equipment equipment)
        {
            try
            {
            var response = await _client.PutAsJsonAsync($"api/v1/Equipment/{id}", equipment);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
        }
        public async Task<string> DeleteEquipment(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/v1/Equipment/{id}");
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
        }
    }
}
