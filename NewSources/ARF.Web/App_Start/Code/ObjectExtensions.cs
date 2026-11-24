namespace ARF.Web.App_Start.Extensions
{
    public static class ObjectExtensions
    {
    }

    public class AjaxRequestResponse
    {
        public AjaxRequestResponse()
        {
            Status = true;
            Message = string.Empty;
            RedirectURL = string.Empty;
        }
        public bool Status {get; set;}
        public string Message { get; set; }
        public string RedirectURL { get; set; }
    }

}