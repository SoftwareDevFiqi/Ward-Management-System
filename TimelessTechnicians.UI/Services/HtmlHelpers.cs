using Microsoft.AspNetCore.Mvc.Rendering;

namespace TimelessTechnicians.UI.Services
{
    public static class HtmlHelpers
    {
        public static SelectList EnumToSelectList<TEnum>(this IHtmlHelper htmlHelper)
            where TEnum : struct, Enum
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(e => new
            {
                Value = e.ToString(),
                Text = e.ToString() // You can replace this with any custom display name logic
            });

            return new SelectList(values, "Value", "Text");
        }
    }
}
