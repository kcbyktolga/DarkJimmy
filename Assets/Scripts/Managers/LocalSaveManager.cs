using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy
{
    public static class LocalSaveManager
    {
        public static DateTime GetResetTime(string key)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, DateTime.Now);
                return DateTime.Now;
            }
            return Convert.ToDateTime(value);
        }

        public static int GetIntValue(string key)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, 0);
                return 0;
            }
            return Convert.ToInt32(value);
        }
        public static int GetIntValue(string key, int defaultValue)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, defaultValue);
                return defaultValue;
            }

            return Convert.ToInt32(value);
        }
        public static bool GetBoolValue(string key)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, false);
                return false;
            }
                
            return Convert.ToBoolean(value);
        }
        public static bool GetBoolValue(string key, bool defaultValue)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, defaultValue);
                return defaultValue;
            }
            return Convert.ToBoolean(value);
        }
        public static float GetFloatValue(string key)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, 0);
                return 0;
            }
            return Convert.ToSingle(value);
        }
        public static float GetFloatValue(string key, float defaultValue)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
            {
                Save(key, defaultValue);
                return defaultValue;
            }
            return Convert.ToSingle(value);
        }
        public static void Save<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
        static string Load(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key);

            return PlayerPrefs.GetString(key, string.Empty);
        }
        public static string GetToggleName(UI.VolumeType type)
        {
            return $"{type} Toogle";
        }
        public static string GetSliderName(UI.VolumeType type)
        {
            return $"{type} Slider";
        }

    }
}

