﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Objects.DataClasses;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TchillrREST
{
    public class Utilities
    {

#warning jsonignore Occurences-Rubriques-Keywords in Activity, Theme-WordClouds-UserTags in Tags, Activity in Occurence

        #region CONST
        public const short RUBRIQUE_WEIGHT = 6;
        public const string DATE_TIME_FORMAT = "yyyyMMddHHmmss";
        public static CultureInfo FRENCH_CULTURE = new CultureInfo("fr-FR");
        public static CultureInfo ENGLISH_CULTURE = new CultureInfo("en-US");
        public const string TRANSPORT_BASE_URL = "http://api.navitia.io/v0/paris/journeys.json?origin={0}&destination={1}&datetime={2}";
        public const string NEARBY_PLACE_BASE_URL = "http://api.navitia.io/v0/paris/places_nearby.json?uri=coord:{0}&count=1";
        public const string CONNEXION_STRING = "Data Source=tcp:i8iid5vg42.database.windows.net,1433;Initial Catalog=TchillrDataBase;User ID=TchillrSGBD@i8iid5vg42;Password=Tch1llrInTown";
        #endregion

        #region struct
        //public struct Colors
        //{
        //    public static string Musique = "";
        //    public static string Activités = "";
        //    public static string Evenements = "";
        //    public static string Nature = "";
        //    public static string Culture = "";
        //    public static string Spectacles = "";
        //}
        #endregion

        private static TchillrREST.DataModel.Entities context = null;

        public static TchillrREST.DataModel.Entities TchillrContext
        {
            get
            {
                if (context == null)
                {
                    context = new TchillrREST.DataModel.Entities(ConfigurationManager.ConnectionStrings["TchillrDataBaseEntities"].ConnectionString);
                    context.ContextOptions.ProxyCreationEnabled = false;
                }

                return context;
            }
        }

        public static bool SetKeywords(TchillrREST.DataModel.Activity activity)
        {
            activity.cleanActivity();

            List<TchillrREST.DataModel.Keyword> keywords = new List<TchillrREST.DataModel.Keyword>();
            const int MAX_KEYWORDS_RETURNED = 8;


            //this.ActivityContextualTags = new List<string>();

            const int NAME_WEIGHT = 5;
            const int SHORT_DESCRIPTION_WEIGHT = 3;
            const int DESCRIPTION_WEIGHT = 1;

            GetWordsOccurences(activity.name, NAME_WEIGHT, keywords);
            GetWordsOccurences(activity.shortDescription, SHORT_DESCRIPTION_WEIGHT, keywords);
            GetWordsOccurences(activity.description, DESCRIPTION_WEIGHT, keywords);

            //var sortedDict = (from entry in keywords orderby entry.Value descending select entry).Take(MAX_KEYWORDS_RETURNED).ToDictionary(pair => pair.Key, pair => pair.Value);

            //wordCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //return this.Keywords.OrderByDescending(x => x.Hits).Take(MAX_KEYWORDS_RETURNED).ToList<Keyword>();
            if (activity.Keywords.Count > 0)
            {
                activity.Keywords.ToList().ForEach(keyword => TchillrContext.Keywords.DeleteObject(keyword));
            }
            
            foreach (TchillrREST.DataModel.Keyword keyword in keywords.OrderByDescending(key => key.hits).Take(MAX_KEYWORDS_RETURNED).ToList<TchillrREST.DataModel.Keyword>())
                activity.Keywords.Add(keyword);

            return true;
        }

        private static List<TchillrREST.DataModel.Keyword> GetWordsOccurences(string text, int weight, List<TchillrREST.DataModel.Keyword> keywords)
        {

            List<string> bannedWords = new List<string>();
            bannedWords.AddRange(new string[] { "le", "la", "les", "l'", "un", "une", "des", "d'", "du", "de", "au", "aux", "ce", "cet", "cette", "ces", "ses", "mon", "ton", "son", "ma", "ta", "sa", 
                                                "mes", "tes", "notre", "votre", "leur", "vötre", "nötre", "leurs", "quel", "quelle", "quels", "quelles", "et", "je","tu","il","elle","ils","elles","nous",
                                                "vous","et","se","avec","en","à","&","","donc","qui",":","par","a","sur","lui","pour","plus",";","dans","d'un","-","dans","celui","tous","tout","que","-",
                                                "!","?",".","nos","beaucoup"
                                                });

            if (!string.IsNullOrEmpty(text))
            {
                //List<string> words = text.Split(' ').ToList<string>().Select(x => x.ToLower()).ToList<string>();
                List<string> words = GetWords(text).Select(x => x.ToLower()).ToList<string>();

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
                        keyword.title = word.ToLower();
                        keyword.hits = weight;

                        keywords.Add(keyword);
                    }
                }
            }
            keywords.ForEach(keyword => keyword.title.ToUpper());
            return keywords;
        }

        public static TchillrREST.DataModel.User GetUserByID(Guid userID)
        {
            return TchillrREST.Utilities.TchillrContext.Users.FirstOrDefault(user => user.identifier == userID);
        }

        public static List<string> GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w]*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select TrimSuffix(m.Value);

            return words.ToList();

        }

        public static string TrimSuffix(string word)
        {
            int apostrapheLocation = word.IndexOf('\'');
            if (apostrapheLocation != -1)
            {
                word = word.Substring(0, apostrapheLocation);

            }

            return word;
        }

        public static string GetForbidenUrls(string excludeTransportMode)
        {
            string result = string.Empty;
            int transportMode = int.Parse(excludeTransportMode);

            // bus
            if ((transportMode >> 0)  % 2 == 1)
                result += "&forbidden_uris[]=commercial_mode:0x3";
            
            // Metro
            if ((transportMode >> 1) % 2 == 1)
                result += "&forbidden_uris[]=commercial_mode:0x1";

            // Tram
            if ((transportMode >> 2) % 2 == 1)
                result += "&forbidden_uris[]=commercial_mode:0x0";
            
            // Rail
            if ((transportMode >> 3) % 2 == 1)
                result += "&forbidden_uris[]=commercial_mode:0x2";

            return result;
        }
    }
}
