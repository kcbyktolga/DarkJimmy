using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;
using System.IO;
using System.Linq;


namespace DarkJimmy
{
    public static class LanguageManager
    {
        public static Language currentLanguage;
        public static bool canChanged;
        static bool initialize;

        public delegate void OnChangedLanguage();
        public static OnChangedLanguage onChangedLanguage;

        private static string[] allwords;

        private static  List<string> lines = new List<string>();
        private static int langCount;
        private static List<string> words = new List<string>();
        private static List<string> langID = new List<string>();


        private static readonly Dictionary<string, string> German = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobby"},
        //    {"Settings","Settings"},
        //    {"Shop","Shop"},
        //    {"Characters","Characters"},
        //    {"Stages","Stages"},
        //    {"Victory","Victory"},
        //    {"Defeat","Defeat"},
        //    {"Play","Play"},
        //    {"Pause","Pause"},
        //    {"PlayService","PlayService"},
        //    {"PrivacyPolicy","PrivacyPolicy"},
        //    {"Languages","Languages"},
        //    {"Credits","Credits"},
        //};
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobby"},
        //    {"Settings","Settýngs"},
        //    {"Shop","Shop"},
        //    {"Characters","Characters"},
        //    {"Stages","Stages"},
        //    {"Victory","Výctory"},
        //    {"Defeat","Defeat"},
        //    {"Play","Play"},
        //    {"Pause","Pause"},
        //    {"PlayService","Play Service"},
        //    {"PrivacyPolicy","Privacy Policy"},
        //    {"Languages","Languages"},
        //    {"Credits","Credits"},

        //     //Others
        //    {"Free","Free"},
        //    {"Go","Go"},
        //    {"Update","Update"},
        //    {"Okay","Okay"},

        //    //Shop 
        //    {"Gold","Gold"},
        //    {"Diamond","Dýamond"},
        //    {"Diamonds","Dýamonds"},
        //    {"Golds","Golds"},
        //    {"Offers","Offers"},
        //    {"Premium","Premýum"},
        //    {"Costumes","Costumes"},



        //    //Popups
        //    {"PurchaseProcess","Purchase failed."},
        //    {"Waiting","Waiting."},
        //    {"StageUpProcess","Credits"},
        //    {"ConnectedError","Connected Error!"},
        //    {"Disconnect","Disconnect"},
        //    {"AppUpdate","New update available, Please update the app."},
        //    {"ShopOrientation","Your balance is not enough. Would you like to go to the store?"},
        //    {"StageLockOrientation","Unlock previous adventure first!"}
        //};
        private static readonly Dictionary<string, string> French = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobby"},
        //    {"Settings","Settings"},
        //    {"Shop","Shop"},
        //    {"Characters","Characters"},
        //    {"Stages","Stages"},
        //    {"Victory","Victory"},
        //    {"Defeat","Defeat"},
        //    {"Play","Play"},
        //    {"Pause","Pause"},
        //    {"PlayService","PlayService"},
        //    {"PrivacyPolicy","PrivacyPolicy"},
        //    {"Languages","Languages"},
        //    {"Credits","Credits"},
        //};
        private static readonly Dictionary<string, string> Italian = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobby"},
        //    {"Settings","Settings"},
        //    {"Shop","Shop"},
        //    {"Characters","Characters"},
        //    {"Stages","Stages"},
        //    {"Victory","Victory"},
        //    {"Defeat","Defeat"},
        //    {"Play","Play"},
        //    {"Pause","Pause"},
        //    {"PlayService","PlayService"},
        //    {"PrivacyPolicy","PrivacyPolicy"},
        //    {"Languages","Languages"},
        //    {"Credits","Credits"},
        //};
        private static readonly Dictionary<string, string> Spanish = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobby"},
        //    {"Settings","Settings"},
        //    {"Shop","Shop"},
        //    {"Characters","Characters"},
        //    {"Stages","Stages"},
        //    {"Victory","Victory"},
        //    {"Defeat","Defeat"},
        //    {"Play","Play"},
        //    {"Pause","Pause"},
        //    {"PlayService","PlayService"},
        //    {"PrivacyPolicy","PrivacyPolicy"},
        //    {"Languages","Languages"},
        //    {"Credits","Credits"},
        //};
        private static readonly Dictionary<string, string> Turkish = new Dictionary<string, string>();
        //{   //Main Menu
        //    {"Lobby","Lobi"},
        //    {"Settings","Ayarlar"},
        //    {"Shop","Market"},
        //    {"Characters","Karakterler"},
        //    {"Stages","Asamalar"},
        //    {"Victory","Zafer"},
        //    {"Defeat","Bozgun"},
        //    {"Play","Oyna"},
        //    {"Pause","Duraklat"},
        //    {"PlayService","Play Servis"},
        //    {"PrivacyPolicy","Gizlilik Politikasý"},
        //    {"Languages","Diller"},
        //    {"Credits","Krediler"},

        //    //Others
        //    {"Free","Ücretsiz"},
        //    {"Go","Git"},
        //    {"Update","Güncelle"},
        //    {"Okay","Tamam"},

        //    //Shop
        //    {"Gold","Altýn"},
        //    {"Diamond","Elmas"},
        //    {"Diamonds","Elmaslar"},
        //    {"Golds","Altýnlar"},
        //    {"Offers","Teklýfler"},
        //    {"Premium","Premýum"},
        //    {"Costumes","Kostumler"},

        //    //Popups
        //    {"PurchaseProcess","Satýn alma baþarýsýz oldu."},
        //    {"Waiting","Lütfen bekleyin.."},
        //    {"StageUpProcess","Credits"},
        //    {"ConnectedError","Baðlantý Hatasý!"},
        //    {"Disconnect","Baðlantý yok!"},
        //    {"AppUpdate","Yeni güncelleme mevcut. Lütfen uygulamayý güncelleyin."},
        //    {"ShopOrientation","Bakiyeniz yeterli deðil. Maðazaya gitmek istiyor musunuz?"},
        //    {"StageLockOrientation","Ýlk olarak önceki macerayý aç!"}
        //};
        private static Dictionary<string, string> GetCurrentDictionary(Language language)
        {
            return language switch
            {
                Language.English => English,
                Language.Spanish => Spanish,
                Language.French => French,
                Language.Italian => Italian,  
                Language.Turkish => Turkish,
                _ => German,
            };
        }

        private static void WordAdd(Language currentLang, string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            if (GetCurrentDictionary(currentLang).ContainsKey(key))
                return;
          
            GetCurrentDictionary(currentLang).Add(key,value);

        }
        private static bool Initialize()
        {
            string key = PlayerPrefs.GetString("Initialize");

            if (string.IsNullOrEmpty(key))
                return false;
            else
                return Convert.ToBoolean(key);
        }
        public static Language GetLanguage()
        {
            string key = PlayerPrefs.GetString("Language");
            Enum.TryParse(key, out Language language);
            return language;
        }
        public static string GetText(string key)
        {
            if (!GetCurrentDictionary(GetLanguage()).ContainsKey(key))
                return key.Trim();
            else
                return GetCurrentDictionary(GetLanguage())[key].Trim();
        }
        public static void DefaultLanguage()
        {
            if (!Initialize())
            {
                if (Enum.TryParse(Application.systemLanguage.ToString(), out currentLanguage))
                    SetLanguage(currentLanguage);

                initialize = true;
                PlayerPrefs.SetString("Initialize", initialize.ToString());
            }
            else
                SetLanguage(GetLanguage());

        }
        public static void SetLanguage(Language language)
        {
            PlayerPrefs.SetString("Language", language.ToString());          
        }
        public static string GetLanguageName(string name)
        {
            return name switch
            {
                nameof(Language.German) => "Deutsch",
                nameof(Language.Spanish) => "Español",
                nameof(Language.French) => "Français",
                nameof(Language.Italian) => "Italiano",
                nameof(Language.Turkish) => "Türkçe",
                _ => "English",
            };

        }

        static LanguageManager()
        {
            langCount = Enum.GetValues(typeof(Language)).Length;

            for (int i = 0; i < langCount; i++)
            {
                ReadCsv(i);
            }
        }


        private static void ReadCsv(int index)
        {
            // textFile = (TextAsset)Resources.Load("Assets/Resources/BSPLOC.txt");

            //TextAsset asset = (TextAsset)Resources.Load("BSPLOC");

            TextAsset mytxtData = (TextAsset)Resources.Load("Localization");
            string txt = mytxtData.text;

            // string readFromFilePath = Application.streamingAssetsPath + "/BSPLOC" + ".txt";*/

            // string fileText = File.ReadAllText(asset.text);

            allwords = txt.Split(',');


           // selectedLaungee = selectedLang;
            for (int i = 0; i < allwords.Length; i += langCount + 1)
            {
                //IEnumerable<string> secondFiveItems = allwords.Skip(i+1).Take(langCount+1);

                //Selected Languages
                IEnumerable<string> secondFiveItems = allwords.Skip(index + 1 + i + 1).Take(1);

                lines = secondFiveItems.ToList();
                string temp = null;
                for (int j = 0; j < secondFiveItems.Count(); j++)
                {
                    temp += lines[j];
                }
                words.Add(temp);
              

                //English Languages
                IEnumerable<string> secondFiveItemsEng = allwords.Skip(i + 2).Take(1);

                lines = secondFiveItemsEng.ToList();
                string tempEng = null;
                for (int j = 0; j < secondFiveItemsEng.Count(); j++)
                {
                    tempEng += lines[j];
                }
               // engWords.Add(tempEng);

                //Selected Languages ID
                IEnumerable<string> secondFiveItemsID = allwords.Skip(i + 1).Take(1);

                lines = secondFiveItemsID.ToList();
                string tempID = null;

                for (int j = 0; j < secondFiveItemsID.Count(); j++)
                {
                    tempID += lines[j];
                }
                
                langID.Add(tempID);

                WordAdd((Language)index,tempID,temp);

               //  GetCurrentDictionary(currentlanguage).Add(tempID,temp);

            }
        }
    }
    public enum Language
    {
        German,
        English,
        Spanish,
        French,
        Italian,
        Turkish,
    }
}

