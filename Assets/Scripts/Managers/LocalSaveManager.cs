using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy
{
    public static class LocalSaveManager
    {

        public static int GetIntValue(string key)
        {
            string value = Load(key);
            return string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
        }
        public static int GetIntValue(string key, int defaultValue)
        {
            string value = Load(key);
            int _value = string.IsNullOrEmpty(value) ? defaultValue : Convert.ToInt32(value);

            Save(key, _value);

            return _value;
        }
        public static bool GetBoolValue(string key)
        {
            string value = Load(key);

            if (string.IsNullOrEmpty(value))
                return false;

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
            return string.IsNullOrEmpty(value) ? 0 : Convert.ToSingle(value);
        }
        public static float GetFloatValue(string key, float defaultValue)
        {
            string value = Load(key);
            return string.IsNullOrEmpty(value) ? defaultValue : Convert.ToSingle(value);
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

