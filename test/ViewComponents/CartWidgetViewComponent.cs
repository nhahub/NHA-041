namespace test.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;

    public class CartWidgetViewComponent : ViewComponent
    {
        // The HttpContextAccessor gives us access to the Session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartWidgetViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // This 'Invoke' method is called when you use the component
        public IViewComponentResult Invoke()
        {
            // 1. Read the count from the Session
            // 2. Use '?? 0' to default to 0 if the session key doesn't exist
            int cartCount = _httpContextAccessor.HttpContext.Session.GetInt32("CartCount") ?? 0;

            // 3. Pass the count (int) to the component's view
            return View(cartCount);
        }
    }
}
