using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TXM.Core
{
    [Serializable]
    public class Language
    {
        private Dictionary<string, string> translation;

        public Language()
        {
            translation = new Dictionary<string, string>();
        }

        public string GetTranslation(string text)
        {
            if (translation.ContainsKey(text))
                return translation[text];
            else
                return text;
        }

        public void SetTranslations(List<string> fileData)
        {
            string[] s;
            translation = new Dictionary<string, string>();
            foreach (var a in fileData)
            {
                s = a.Split(':');
                translation.Add(s[0], s[1]);
            }
        }
    }
}
