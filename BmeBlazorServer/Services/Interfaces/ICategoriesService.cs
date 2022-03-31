using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface ICategoryService
    {
        Task<bool> InitializeService();
        Task<List<Category>> GetCategories();
        Task<HttpResponseMessage> DeleteCategory(int categoryId);
        Task<HttpResponseMessage> UpdateCategory(Category category);
        event Action? OnChange;
    }
}
