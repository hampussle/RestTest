using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DemoApi.Services.Extensions
{
    public static class Extension
    {
        public static string GetUserId(this HttpContext ctx)
        {
            return ctx.User != null ? ctx.User.Claims.Single(x => x.Type == "id").Value : string.Empty;
        }
    }
}