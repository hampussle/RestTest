using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApi.Data.Repositories;
using DemoApi.Models;
using DemoApi.Models.Posts;

namespace DemoApi.Services
{
    public class PostService : IPostService
    {
        protected IAsyncRepository<Post> Repo;

        public PostService(IAsyncRepository<Post> repo)
        {
            Repo = repo;
        }

        public async Task<PostResponse> DeleteAsync(Guid id)
        {
            Post post = await Repo.GetByIdAsync(id);
            if (post is null)
                return new PostResponse($"Post with id: {id} wasn't found");
            Repo.Delete(post);
            int result = await Repo.SaveAsync();
            return result == 0
                ? new PostResponse("No changes were made")
                : new PostResponse(post: post);
        }

        public async Task<PostResponse> UpdateAsync(Guid id, string content)
        {
            Post post = await Repo.GetByIdAsync(id);
            if (post is null)
                return new PostResponse($"Post with id {id} wasn't found");
            string beforeUpdate = post.Content;
            post.Content = content;
            Repo.Update(post);
            int result = await Repo.SaveAsync();
            return result == 0
                ? new PostResponse("No changes were made")
                : new PostResponse($"{beforeUpdate} was changed to: {post.Content}");
        }

        public async Task<PostResponse> GetByIdAsync(Guid id)
        {
            Post post = await Repo.GetByIdAsync(id);
            return post is null
                ? new PostResponse($"Post with id: {id} wasn't found")
                : new PostResponse(post: post);
        }

        public async Task<PostResponse> AddAsync(string userid, string content)
        {
            Post post = new()
            {
                Content = content,
                UserId = userid
            };
            await Repo.AddAsync(post);
            int result = await Repo.SaveAsync();
            return result == 0
                ? new PostResponse("No changes were made")
                : new PostResponse(post: post);
        }

        public async Task<PostResponse> GetAllAsync()
        {
            List<Post> postList = await Repo.GetAllAsync();
            return postList.Count == 0
                ? new PostResponse("No content was found")
                : new PostResponse(postList: postList);
        }

        public async Task<bool> CorrectUserForPost(Guid id, string userId)
        {
            Post post = await Repo.GetByIdAsync(id);
            return post is not null && post.UserId == userId;
        }
    }
}