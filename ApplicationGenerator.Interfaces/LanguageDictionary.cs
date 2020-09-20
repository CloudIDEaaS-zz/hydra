using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeInterfaces;
using Utils;

namespace AbstraX
{
    public class LanguageDictionary
    {
        public Dictionary<string, LanguageSpecificDictionary> LanguageSpecificDictionaries { get; }

        public LanguageDictionary()
        {
            this.LanguageSpecificDictionaries = new Dictionary<string, LanguageSpecificDictionary>();
        }

        public string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame)
        {
            var culture = CultureInfo.CurrentCulture;
            var dictionary = this.LanguageSpecificDictionaries.AddToDictionaryIfNotExist(culture.TwoLetterISOLanguageName);

            dictionary.LanguageCode = culture.TwoLetterISOLanguageName;

            return dictionary.AddTranslation(baseObject, key, value, skipIfSame);
        }
    }

    public class LanguageSpecificDictionary
    {
        public string LanguageCode { get; set; }
        public Dictionary<string, LanguageTranslations> LanguageTranslationsGroups { get; }
        public Dictionary<string, string> AllTranslations { get; }

        public LanguageSpecificDictionary()
        {
            this.LanguageTranslationsGroups = new Dictionary<string, LanguageTranslations>();
        }

        public string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame)
        {
            var leaf = baseObject.GetNavigationLeaf();
            var originalKey = key;
            var translations = this.LanguageTranslationsGroups.AddToDictionaryIfNotExist(leaf, new LanguageTranslations());
            var allTranslations = this.LanguageTranslationsGroups.Values.SelectMany(g => g).ToDictionary(g => g.Key, g => g.Value);
            var x = 1;

            if (allTranslations.ContainsKey(key) && skipIfSame)
            {
                var existingValue = allTranslations[key];

                if (existingValue == value)
                {
                    return key;
                }
            }
            
            while (allTranslations.ContainsKey(key))
            {
                key = string.Format("{0}_{1}", originalKey, x++);
            }

            translations.Add(key, value);

            return key;
        }
    }

    public class LanguageTranslations : BaseDictionary<string, string>
    {
        private Dictionary<string, string> translations;

        public override int Count => translations.Count;

        public LanguageTranslations()
        {
            translations = new Dictionary<string, string>();
        }

        public override void Add(string key, string value)
        {
            translations.Add(key, value);
        }

        public override void Clear()
        {
            translations.Clear();
        }

        public override bool ContainsKey(string key)
        {
            return translations.ContainsKey(key);
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return translations.Select(t => new KeyValuePair<string, string>(t.Key, t.Value)).GetEnumerator();
        }

        public override bool Remove(string key)
        {
            return translations.Remove(key);
        }

        public override bool TryGetValue(string key, out string value)
        {
            return translations.TryGetValue(key, out value);
        }

        protected override void SetValue(string key, string value)
        {
            translations[key] = value;
        }
    }
}
