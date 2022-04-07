using BmeModels;

namespace BmeBlazorServer.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> InitializeService();
        Task<List<Subcategory>> GetSubCategories();
        Task<List<Category>> GetCategories();
        Task<HttpResponseMessage> DeleteCategory(int categoryId);
        Task<HttpResponseMessage> UpdateCategory(Category category);
        event Action? OnChange;
    }
}
