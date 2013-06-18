﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TchillrREST
{
    public class Utilities
    {
        private static TchillrREST.DataModel.Entities context = null;

        public static TchillrREST.DataModel.Entities TchillrContext
        {
            get
            {
                if( context == null){
                    context = new TchillrREST.DataModel.Entities(ConfigurationManager.ConnectionStrings["TchillrDataBaseEntities"].ConnectionString);
                    context.ContextOptions.ProxyCreationEnabled = false;
                }

                return context;
            }
        }

        public static bool SetKeywords(TchillrREST.DataModel.Activity activity, List<string> tags)
        {
            List<TchillrREST.DataModel.Keyword> keywords = new List<TchillrREST.DataModel.Keyword>();
            const int MAX_KEYWORDS_RETURNED = 8;

            
            //this.ActivityContextualTags = new List<string>();

            const int NAME_WEIGHT = 5;
            const int SHORT_DESCRIPTION_WEIGHT = 3;
            const int DESCRIPTION_WEIGHT = 1;

            GetWordsOccurences(activity.name, NAME_WEIGHT, tags, keywords);
            GetWordsOccurences(activity.shortDescription, SHORT_DESCRIPTION_WEIGHT, tags, keywords);
            GetWordsOccurences(activity.description, DESCRIPTION_WEIGHT, tags, keywords);

            //var sortedDict = (from entry in keywords orderby entry.Value descending select entry).Take(MAX_KEYWORDS_RETURNED).ToDictionary(pair => pair.Key, pair => pair.Value);

            //wordCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //return this.Keywords.OrderByDescending(x => x.Hits).Take(MAX_KEYWORDS_RETURNED).ToList<Keyword>();
            foreach (TchillrREST.DataModel.Keyword keyword in keywords.OrderByDescending(key => key.hits).Take(MAX_KEYWORDS_RETURNED).ToList<TchillrREST.DataModel.Keyword>())
                activity.Keywords.Add(keyword);

            return true;
        }

        private static List<TchillrREST.DataModel.Keyword> GetWordsOccurences(string text, int weight, List<string> tags, List<TchillrREST.DataModel.Keyword> keywords)
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

                    if (keywords.Select(key => key.title).Contains(word))
                    {
                        keywords.Where(key => key.title == word).First().hits += weight;
                    }
                    else
                    {
                        TchillrREST.DataModel.Keyword keyword = new TchillrREST.DataModel.Keyword();
                        keyword.title = word;
                        keyword.hits = weight;

                        keywords.Add(keyword);
                    }
                    //Keyword keyWord = this.Keywords.FirstOrDefault( key=> key.Title == word);
                    //if (keyWord == null || string.IsNullOrEmpty(keyWord.Title))
                    //{
                    //    keyWord = new Keyword();
                    //    keyWord.Hits = weight;
                    //    keyWord.Title = word;
                    //    keyWord.ActivityID = this.ID;
                    //    this.Keywords.Add(keyWord);
                    //}
                    //else
                    //{
                    //    keyWord.Hits += weight;
                    //}

                    //if (tags.Contains(word.ToUpper()))
                    //    if (this.ActivityContextualTags.Contains(word.ToUpper()))
                    //        continue;
                    //    else
                    //        this.ActivityContextualTags.Add(word.ToUpper());
                }
            }
            //return this.Keywords;
            return keywords;
        }

    }
}