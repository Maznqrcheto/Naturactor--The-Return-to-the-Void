using UnityEngine;
using System.Collections.Generic;
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> Achievements = new List<Achievement>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockAchievement(string title)
    {
        Achievement achievement = null;
        for (int i = 0; i < Achievements.Count; i++)
        {
            if (Achievements[i].Title == title)
                {
                    achievement = Achievements[i];
                    break;
                }
        }

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            Debug.Log("Achievement Unlocked: " + achievement.Title);
        }
    }

    public bool IsAchievementUnlocked(string title)
    {
        for (int i = 0; i < Achievements.Count; i++)
        {
            if (Achievements[i].Title == title)
            {
                return Achievements[i].IsUnlocked;
            }
        }

        return false;
    }
}