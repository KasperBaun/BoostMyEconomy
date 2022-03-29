using BmeModels;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Category> Categories { get; set; } = new();
        public event Action OnChange;

        public CategoriesService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

        /* Get all categories */
        public async Task<List<Category>> GetCategories()
        {
            if(Categories == null)
            {
                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Categories/All");
                    var token =  await localStorageService.GetItemAsync<string>("token");
                    requestMessage.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.SendAsync(requestMessage);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return Categories =  await Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(responseBody));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }

            return Categories;
        }

        /* Delete category */
        public async Task<HttpResponseMessage> DeleteCategory(int categoryId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri: "api/Categories/"+ categoryId);
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            return response;
        }

        /* Update category */
        public async Task<HttpResponseMessage> UpdateCategory(Category category)
        {   /*
            var response = await httpClient.PutAsJsonAsync("api/Users", user);
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
            */
            return await httpClient.PutAsJsonAsync("api/Categories/", category);
        }

    }
}
