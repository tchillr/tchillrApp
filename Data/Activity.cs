﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchillrREST.Data
{
    [DataContract]
    public class Activity
    {
        [DataMember(Name = "identifier")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

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

        [DataMember(Name = "activityContextualTags")]
        public List<string> ActivityContextualTags { get; set; }

        [DataMember(Name = "keywords")]
        public Dictionary<string, int> Keywords { get; set; }

        public Dictionary<string, int> GetKeywords(List<string> tags)
        {
            const int MAX_KEYWORDS_RETURNED = 8;

            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            this.ActivityContextualTags = new List<string>();

            const int NAME_WEIGHT = 5;
            const int SHORT_DESCRIPTION_WEIGHT = 3;
            const int DESCRIPTION_WEIGHT = 1;

            GetWordsOccurences(this.Nom, NAME_WEIGHT, wordCount, tags);
            GetWordsOccurences(this.ShortDescription, SHORT_DESCRIPTION_WEIGHT, wordCount, tags);
            GetWordsOccurences(this.Description, DESCRIPTION_WEIGHT, wordCount, tags);

            var sortedDict = (from entry in wordCount orderby entry.Value descending select entry).Take(MAX_KEYWORDS_RETURNED).ToDictionary(pair => pair.Key, pair => pair.Value);

            //wordCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return sortedDict;
        }

        private Dictionary<string, int> GetWordsOccurences(string text, int weight, Dictionary<string, int> wordCount, List<string> tags)
        {

            List<string> bannedWords = new List<string>();
            bannedWords.AddRange(new string[] { "le", "la", "les", "l'", "un", "une", "des", "d'", "du", "de", "au", "aux", "ce", "cet", "cette", "ces", "ses", "mon", "ton", "son", "ma", "ta", "sa", 
                                                "mes", "tes", "notre", "votre", "leur", "vötre", "nötre", "leurs", "quel", "quelle", "quels", "quelles", "et", "je","tu","il","elle","ils","elles","nous",
                                                "vous","et","se","avec","en","à","&","","donc","qui",":","par","a","sur","lui","pour","plus",";","dans","d'un","-","dans","celui","tous","tout","que","-",
                                                "!","?",".","nos","beaucoup"
                                                });

            if (!string.IsNullOrEmpty(text))
            {
                List<string> words = text.Split(' ').ToList<string>().Select(x => x.ToLower()).ToList<string>();
                foreach (string word in words)
                {
                    if (bannedWords.Contains(word) || word.Length <= 2)
                        continue;

                    if (wordCount.ContainsKey(word))
                        wordCount[word] += weight;
                    else
                        wordCount.Add(word, weight);

                    if (tags.Contains(word.ToUpper()))
                        if (this.ActivityContextualTags.Contains(word.ToUpper()))
                            continue;
                        else
                            this.ActivityContextualTags.Add(word.ToUpper());
                }
            }
            return wordCount;
        }

    }
}