using BmeModels;
using Newtonsoft.Json;

namespace BmeBlazorServer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Category> Categories { get; set; } = new();
        private List<Subcategory> SubCategories { get; set; } = new();
        public event Action? OnChange;

        public CategoryRepository(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

        /* Get all categories */
        public async Task<List<Category>> GetCategories()
        {
            if(!Categories.Any())
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
                        var categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
                        if(categories != null)
                        {
                            Categories = categories;
                            OnChange?.Invoke();
                            return categories;
                        }
                        else
                        {
                            return Categories;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Categories;
        }

        /* Get all subcategories */
        public async Task<List<Subcategory>> GetSubCategories()
        {
            if (!SubCategories.Any())
            {
                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/SubCategories/All");
                    var token = await localStorageService.GetItemAsync<string>("token");
                    requestMessage.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.SendAsync(requestMessage);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var subcategories = JsonConvert.DeserializeObject<List<Subcategory>>(responseBody);
                        if (subcategories != null)
                        {
                            SubCategories = subcategories;
                            OnChange?.Invoke();
                            return subcategories;
                        }
                        else
                        {
                            return subcategories;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return SubCategories;
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

        public Task<bool> InitializeService()
        {
            throw new NotImplementedException();
        }
    }
}
