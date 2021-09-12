using System;
using System.Threading.Tasks;
using DemoApi.Models.Posts;

namespace DemoApi.Services
{
    public interface IPostService
    {
        Task<PostResponse> DeleteAsync(Guid id);
        Task<PostResponse> UpdateAsync(Guid id, string content);
        Task<PostResponse> GetByIdAsync(Guid id);
        Task<PostResponse> AddAsync(string userid, string content);
        Task<PostResponse> GetAllAsync();
        Task<bool> CorrectUserForPost(Guid id, string userId);
    }
}