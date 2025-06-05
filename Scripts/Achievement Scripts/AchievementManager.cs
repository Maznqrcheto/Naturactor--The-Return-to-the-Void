using UnityEngine;
using System.Collections.Generic;
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> Achievements = new List<Achievement>();

    public void UnlockAchievement(string title)
    {
        Achievement achievement = Achievements.Find(a => a.Title == title);
        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            Debug.Log($"Achievement Unlocked: {achievement.Title}");
        }
    }

    public bool IsAchievementUnlocked(string title)
    {
        Achievement achievement = Achievements.Find(a => a.Title == title);
        return achievement != null && achievement.IsUnlocked;
    }
}