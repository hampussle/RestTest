using System.Threading.Tasks;
using DemoApi.Services;
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
            var response = await _postService.GetAllAsync();
            if (response.PostList is null)
                return NotFound(response.Message);
            return Ok(response.PostList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _postService.GetByIdAsync(id);
            if (response.Post is null)
                return NotFound(response.Message);
            return Ok(response.Post);
        }

        [HttpPost("{content}")]
        public async Task<IActionResult> AddAsync(string content)
        {
            var response = await _postService.AddAsync(content);
            if (response.Post is null)
                return NotFound(response.Message);
            return Ok(response.Post);
        }

        [HttpPut("{id, content}")]
        public async Task<IActionResult> UpdateAsync(int id, string content)
        {
            var response = await _postService.UpdateAsync(id, content);
            if (response.Post is null)
                return NotFound(response.Message);
            return Ok(response.Post);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _postService.DeleteAsync(id);
            if (response.Post is null)
                return NotFound(response.Message);
            return Ok(response.Post);
        }
    }
}