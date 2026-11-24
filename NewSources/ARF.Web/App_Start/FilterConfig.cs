using System.Web.Mvc;

namespace ARF.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new NonceFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}