using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static void SaveDataInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
    public static void SaveDataFloat (string key, float value)
    {
        PlayerPrefs.SetFloat (key, value);
        PlayerPrefs.Save();
    }
    public static void SaveDataString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static int LoadDataInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
    public static float LoadDataFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }
    public static string LoadDataString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    public static void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
