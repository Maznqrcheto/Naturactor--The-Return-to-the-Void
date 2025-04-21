using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Slider progressBar;
    private float currentLevel = 100f;

    public void SetProgress(float value)
    {
        currentLevel = Mathf.Clamp(value, 0, 100);
        if (progressBar != null)
        {
            progressBar.value = currentLevel;
        }
    }

    public float GetProgress()
    {
        return currentLevel;
    }

    private void Start()
    {
        //fdkfdk
    }

}
