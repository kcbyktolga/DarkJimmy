using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy
{
    public static class LanguageManager
    {
        public static Language currentLanguage;
        public static bool canChanged;
        static bool initialize;

        public delegate void OnChangedLanguage();
        public static OnChangedLanguage onChangedLanguage;

        private static readonly Dictionary<string, string> German = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobby"},
            {"Settings","Settings"},
            {"Shop","Shop"},
            {"Characters","Characters"},
            {"Stages","Stages"},
            {"Victory","Victory"},
            {"Defeat","Defeat"},
            {"Play","Play"},
            {"Pause","Pause"},
            {"PlayService","PlayService"},
            {"PrivacyPolicy","PrivacyPolicy"},
            {"Languages","Languages"},
            {"Credits","Credits"},
        };
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobby"},
            {"Settings","Settýngs"},
            {"Shop","Shop"},
            {"Characters","Characters"},
            {"Stages","Stages"},
            {"Victory","Výctory"},
            {"Defeat","Defeat"},
            {"Play","Play"},
            {"Pause","Pause"},
            {"PlayService","Play Service"},
            {"PrivacyPolicy","Privacy Policy"},
            {"Languages","Languages"},
            {"Credits","Credits"},
        };
        private static readonly Dictionary<string, string> French = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobby"},
            {"Settings","Settings"},
            {"Shop","Shop"},
            {"Characters","Characters"},
            {"Stages","Stages"},
            {"Victory","Victory"},
            {"Defeat","Defeat"},
            {"Play","Play"},
            {"Pause","Pause"},
            {"PlayService","PlayService"},
            {"PrivacyPolicy","PrivacyPolicy"},
            {"Languages","Languages"},
            {"Credits","Credits"},
        };
        private static readonly Dictionary<string, string> Italian = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobby"},
            {"Settings","Settings"},
            {"Shop","Shop"},
            {"Characters","Characters"},
            {"Stages","Stages"},
            {"Victory","Victory"},
            {"Defeat","Defeat"},
            {"Play","Play"},
            {"Pause","Pause"},
            {"PlayService","PlayService"},
            {"PrivacyPolicy","PrivacyPolicy"},
            {"Languages","Languages"},
            {"Credits","Credits"},
        };
        private static readonly Dictionary<string, string> Spanish = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobby"},
            {"Settings","Settings"},
            {"Shop","Shop"},
            {"Characters","Characters"},
            {"Stages","Stages"},
            {"Victory","Victory"},
            {"Defeat","Defeat"},
            {"Play","Play"},
            {"Pause","Pause"},
            {"PlayService","PlayService"},
            {"PrivacyPolicy","PrivacyPolicy"},
            {"Languages","Languages"},
            {"Credits","Credits"},
        };
        private static readonly Dictionary<string, string> Turkish = new Dictionary<string, string>
        {   //Main Menu
            {"Lobby","Lobi"},
            {"Settings","Ayarlar"},
            {"Shop","Market"},
            {"Characters","Karakterler"},
            {"Stages","Asamalar"},
            {"Victory","Zafer"},
            {"Defeat","Bozgun"},
            {"Play","Oyna"},
            {"Pause","Duraklat"},
            {"PlayService","Play Servis"},
            {"PrivacyPolicy","Gizlilik Politikasý"},
            {"Languages","Diller"},
            {"Credits","Krediler"},
        };
        private static Dictionary<string, string> GetDictionary()
        {
            return GetLanguage() switch
            {
                Language.English => English,
                Language.Spanish => Spanish,
                Language.French => French,
                Language.Italian => Italian,  
                Language.Turkish => Turkish,
                _ => German,
            };
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
            if (!GetDictionary().ContainsKey(key))
                return key;
            else
                return GetDictionary()[key];
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

