using System.Threading.Tasks;
using DemoApi.Data.Repositories;
using DemoApi.Models;

namespace DemoApi.Services
{
    public class PostService : IPostService
    {
        protected IAsyncRepository<Post> Repo;

        public PostService(IAsyncRepository<Post> repo)
        {
            Repo = repo;
        }

        public async Task<PostResponse> DeleteAsync(int id)
        {
            var post = await Repo.GetByIdAsync(id);
            if (post is null)
                return new PostResponse($"Post with id: {id} wasn't found");
            Repo.Delete(post);
            var result = await Repo.SaveAsync();
            if (result == 0)
                return new PostResponse("No changes were made");
            return new PostResponse(post: post);
        }

        public async Task<PostResponse> UpdateAsync(int id, string content)
        {
            var post = await Repo.FirstOrDefaultAsync(x => x.Id == id);
            if (post is null)
                return new PostResponse($"Post with id {id} wasn't found");
            var beforeUpdate = post.Content;
            post.Content = content;
            Repo.Update(post);
            var result = await Repo.SaveAsync();
            if (result == 0)
                return new PostResponse("No changes were made");
            return new PostResponse($"{beforeUpdate} was changed to: {post.Content}");
        }

        public async Task<PostResponse> GetByIdAsync(int id)
        {
            var post = await Repo.GetByIdAsync(id);
            if (post is null)
                return new PostResponse($"Post with id: {id} wasn't found");
            return new PostResponse(post: post);
        }

        public async Task<PostResponse> AddAsync(string content)
        {
            Post post = new()
            {
                Content = content
            };
            await Repo.AddAsync(post);
            var result = await Repo.SaveAsync();
            if (result == 0)
                return new PostResponse("No changes were made");
            return new PostResponse(post: post);
        }

        public async Task<PostResponse> GetAllAsync()
        {
            var postList = await Repo.GetAllAsync();
            if (postList.Count == 0)
                return new PostResponse("No content was found");
            return new PostResponse(postList: postList);
        }
    }
}