namespace TimelessTechnicians.UI.Services
{
    public class BreadcrumbMiddleware
    {
        private readonly RequestDelegate _next;

        public BreadcrumbMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/AccessDenied")
            {
                var currentUrl = $"{context.Request.Path}{context.Request.QueryString}";
                var breadcrumbHistory = context.Session.GetString("BreadcrumbHistory");
                breadcrumbHistory = string.IsNullOrEmpty(breadcrumbHistory) ? currentUrl : $"{breadcrumbHistory}|{currentUrl}";
                context.Session.SetString("BreadcrumbHistory", breadcrumbHistory);
            }

            await _next(context);
        }
    }

}
