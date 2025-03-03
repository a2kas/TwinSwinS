namespace TwinsWins.Middleware
{
    public class ImageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _imageDirectory;

        public ImageMiddleware(RequestDelegate next, string imageDirectory)
        {
            _next = next;
            _imageDirectory = imageDirectory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path.StartsWith("/images/"))
            {
                var filePath = Path.Combine(_imageDirectory, path.Substring(8));
                if (File.Exists(filePath))
                {
                    context.Response.ContentType = "image/jpeg";
                    await context.Response.SendFileAsync(filePath);
                    return;
                }
            }

            await _next(context);
        }
    }
}
