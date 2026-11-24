namespace ARF.Core
{
    public abstract class Translator
    {
        public abstract string GetLangValueByString(string str, bool returnNullIfNotFound = false, string lang = null);


        public string this[string str]
        {
            get
            {
                var value = GetLangValueByString(str, false);
                return value;
                

            }
        }
    }
}
