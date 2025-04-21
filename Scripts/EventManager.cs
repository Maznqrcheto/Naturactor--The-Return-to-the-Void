using UnityEngine;

public class EventManager : MonoBehaviour
{
    public ProgressBarController fireBar;
    public ProgressBarController waterBar;
    public ProgressBarController airBar;
    public ProgressBarController earthBar;

    public float fireLevel;
    float waterLevel;
    float airLevel;
    float earthLevel;
    float fireChange;
    float waterChange;
    float airChange;
    float earthChange;

    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();

        if (Mathf.Abs(fireLevel - waterLevel) > 20f)
        {
            Debug.Log("Elements out of balance!");
        }
    }

    void Start()
    {
        fireBar.SetProgress(78f);
        waterBar.SetProgress(40f);
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();
    }
}
