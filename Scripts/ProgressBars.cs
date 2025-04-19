using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarController : MonoBehaviour
{
    public Slider progressBar;
    public void SetProgress(float value)
    {
        progressBar.value = Mathf.Clamp(value, 0, 100);
    }
    void Start()
    {
        progressBar.value = 100f;
    }
}