using System;
using System.Threading.Tasks;
using DemoApi.Models.Posts;
using DemoApi.Services;
using DemoApi.Services.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [ApiController]
    [Route("Post")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            PostResponse response = await _postService.GetAllAsync();
            return response.PostList is null
                ? NotFound(response.Message)
                : Ok(response.PostList);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            PostResponse response = await _postService.GetByIdAsync(id);
            return response.Post is null
                ? NotFound(response.Message)
                : Ok(response.Post);
        }

        [HttpPost("{content}")]
        public async Task<IActionResult> AddAsync(string content)
        {
            PostResponse response = await _postService.AddAsync(HttpContext.GetUserId(), content);
            return response.Post is null
                ? NotFound(response.Message)
                : Ok(response.Post);
        }

        [HttpPut("{id, content}")]
        public async Task<IActionResult> UpdateAsync(Guid id, string content)
        {
            bool permission = await _postService.CorrectUserForPost(id, HttpContext.GetUserId());
            if (!permission)
                return BadRequest(new { error = "User does not own post" });
            PostResponse response = await _postService.UpdateAsync(id, content);
            return response.Post is null
                ? NotFound(response.Message)
                : Ok(response.Post);
        }

        [HttpDelete("delete/{id:Guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            PostResponse response = await _postService.DeleteAsync(id);
            return response.Post is null
                ? NotFound(response.Message)
                : Ok(response.Post);
        }
    }
}