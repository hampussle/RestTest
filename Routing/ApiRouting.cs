namespace DemoApi.Routing
{
    public static class ApiRouting
    {
        private const string Root = "api";
        private const string Version = "v1";
        public const string RootPath = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = RootPath + "/" + "posts";
            public const string GetById = RootPath + "/" + "{id}";
        }

        public static class Users
        {
        }
    }
}