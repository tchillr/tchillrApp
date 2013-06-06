using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TchillrREST.Data
{
    [DataContract]
    public class Activity
    {
        [DataMember(Name = "identifier")]
        public int Idactivites { get; set; }

        [DataMember(Name = "name")]
        public string Nom { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "shortDescription")]
        public string ShortDescription { get; set; }

        [DataMember(Name = "place")]
        public string Lieu { get; set; }

        [DataMember(Name = "adress")]
        public string Adresse { get; set; }

        [DataMember(Name = "zipcode")]
        public string Zipcode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "latitude")]
        public float Lat { get; set; }

        [DataMember(Name = "longitude")]
        public float Lon { get; set; }

        [DataMember(Name = "occurences")]
        public List<Occurence> Occurences { get; set; }

        [DataMember(Name = "keywords")]
        public Dictionary<string, int> Keywords { get; set; }

        public Dictionary<string, int> GetKeywords()
        {
            const int MAX_KEYWORDS_RETURNED = 8;
            
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            const int NAME_WEIGHT = 5;
            const int SHORT_DESCRIPTION_WEIGHT = 3;
            const int DESCRIPTION_WEIGHT = 1;
            
            GetWordsOccurences(this.Nom, NAME_WEIGHT,wordCount);
            GetWordsOccurences(this.ShortDescription, SHORT_DESCRIPTION_WEIGHT, wordCount);
            GetWordsOccurences(this.Description, DESCRIPTION_WEIGHT,wordCount);

            var sortedDict = (from entry in wordCount orderby entry.Value descending select entry).Take(MAX_KEYWORDS_RETURNED).ToDictionary(pair => pair.Key, pair => pair.Value);

            //wordCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return sortedDict;
        }

        private Dictionary<string, int> GetWordsOccurences(string text, int weight, Dictionary<string, int> wordCount)
        {
            List<string> bannedWords = new List<string>();
            bannedWords.AddRange(new string[] { "le", "la", "les", "l'", "un", "une", "des", "d'", "du", "de", "au", "aux", "ce", "cet", "cette", "ces", "ses", "mon", "ton", "son", "ma", "ta", "sa", "mes", "tes", "notre", "votre", "leur", "vötre", "nötre", "leurs", "quel", "quelle", "quels", "quelles", "et" });

            if (!string.IsNullOrEmpty(text))
            {
                List<string> words = text.Split(' ').ToList<string>();
                foreach (string word in words)
                {
                    if (bannedWords.Contains(word))
                        continue;

                    if (wordCount.ContainsKey(word))
                        wordCount[word] += weight;
                    else
                        wordCount.Add(word, weight);
                }
            }
            return wordCount;
        }

    }
}