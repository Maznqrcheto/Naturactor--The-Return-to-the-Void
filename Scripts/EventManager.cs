using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Slide bars")]
    public ProgressBarController fireBar;
    public ProgressBarController waterBar;
    public ProgressBarController airBar;
    public ProgressBarController earthBar;
    public ProgressBarController happinessBar;

    [Header("Element levels")]
    public float fireLevel;
    public float waterLevel;
    public float airLevel;
    public float earthLevel;

    [Header("Happiness level")]
    public float happinessLevel;

    [Header("Element changes")]
    public float fireChange;
    public float waterChange;
    public float airChange;
    public float earthChange;

    [Header("Happiness change")]
    public float happinessChange;

    [Header("Tick system")]
    public ulong ticksToChange = 0;
    public TickSystem tickSystem;

    [Header("Events")]
    public Drought droughtEvent;
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    private void Update()
    {
        if (ticksToChange == 0)
            ticksToChange = tickSystem.tickTime + 30;

        if (ticksToChange <= tickSystem.tickTime)
        {
            waterLevel += waterChange;
            fireLevel += fireChange;
            earthLevel += earthChange;
            airLevel += airChange;
            happinessLevel += happinessChange;
            ticksToChange = 0;
        }

        waterBar.SetProgress(waterLevel);
        fireBar.SetProgress(fireLevel);
        earthBar.SetProgress(earthLevel);
        airBar.SetProgress(airLevel);
        happinessBar.SetProgress(happinessLevel);
    }
    public void GetElementProgress()
    {
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();
        airLevel = airBar.GetProgress();
        earthLevel = earthBar.GetProgress();
        happinessLevel = happinessBar.GetProgress();
    }
    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        GetElementProgress();

        if (Mathf.Abs(fireLevel - waterLevel) > 20f && !droughtEvent.droughtOccured && droughtEvent.droughtCooldown == 2400)
        {
            //drought occurs
            droughtEvent.mapGenerator = mapGenerator;
            droughtEvent.grid = mapGenerator.grid;
            droughtEvent.StartDrought();
        }
    }
    void Start()
    {
        fireBar.SetProgress(100f);
        waterBar.SetProgress(100f);
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();
        happinessBar.SetProgress(100f);
    }
}
