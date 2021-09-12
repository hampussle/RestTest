using System.Collections.Generic;

namespace DemoApi.Models.Posts
{
    public class PostResponse
    {
        public int? Count;
        public string Message;
        public Post Post;
        public List<Post> PostList;

        public PostResponse(string message = null, List<Post> postList = null, Post post = null, int? count = null)
        {
            PostList = postList;
            Post = post;
            Message = message;
            Count = count;
        }
    }
}