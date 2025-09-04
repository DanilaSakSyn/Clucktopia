using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    private const string LastLoginKey = "LastLoginTime";

    public void SaveLastLoginTime()
    {
        DateTime now = DateTime.Now;
        PlayerPrefs.SetString(LastLoginKey, now.ToString());
        PlayerPrefs.Save();
    }

    public float GetTimeSinceLastLogin()
    {
        if (PlayerPrefs.HasKey(LastLoginKey))
        {
            string lastLoginString = PlayerPrefs.GetString(LastLoginKey);
            if (DateTime.TryParse(lastLoginString, out DateTime lastLoginTime))
            {
                TimeSpan timeSinceLastLogin = DateTime.Now - lastLoginTime;
                return (float)timeSinceLastLogin.TotalSeconds;
            }
        }

        // If no last login time is found, return -1 to indicate first login
        return -1f;
    }

    private void OnApplicationQuit()
    {
        SaveLastLoginTime();
    }
}
