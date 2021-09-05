using System.Threading.Tasks;
using DemoApi.Models;

namespace DemoApi.Services
{
    public interface IPostService
    {
        Task<PostResponse> DeleteAsync(int id);
        Task<PostResponse> UpdateAsync(int id, string content);
        Task<PostResponse> GetByIdAsync(int id);
        Task<PostResponse> AddAsync(string content);
        Task<PostResponse> GetAllAsync();
    }
}