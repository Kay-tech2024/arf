using System.Web;

namespace ARF.TimeMachine
{
    internal class TmHttpContextManager
    {
        private static HttpContextBase m_context;

        public static HttpContextBase Current
        {
            get
            {
                if (m_context != null)
                    return m_context;

                if (HttpContext.Current == null)
                    return null;

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            m_context = context;
        }
    }
}