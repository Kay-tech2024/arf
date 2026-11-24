namespace ARF.Web.Code.App_Start
{
    public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            //HangfireBootstrapper.Instance.Start();
        }
    }
}