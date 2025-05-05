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
    public Flood floodEvent;
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    private void Update()
    {
        if (ticksToChange == 0)
        {
            ticksToChange = tickSystem.tickTime + 50;
        }
        if (ticksToChange <= tickSystem.tickTime)
        {
            SetElementalChange();

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
    public void SetElementProgress(value)
    {
        fireBar.SetProgress(value);
        waterBar.SetProgress(value);
        airBar.SetProgress(value);
        earthBar.SetProgress(value);
        happinessBar.SetProgress(value);
    }    
    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        GetElementProgress();

        if (fireLevel - waterLevel > 20f && !droughtEvent.droughtOccured && droughtEvent.droughtCooldown == 2400) // Drought Event
        {
            droughtEvent.mapGenerator = mapGenerator;
            droughtEvent.grid = mapGenerator.grid;
            droughtEvent.StartDrought();
        }
        if (waterLevel - fireLevel > 20f && !floodEvent.floodOccured && floodEvent.floodCooldown == 2400)
        {
            floodEvent.mapGenerator = mapGenerator;
            floodEvent.grid = mapGenerator.grid;
            floodEvent.StartFlood();
        }
    }
    void Start()
    {
        SetElementalProgress(100f);
    }
    public void SetElementalChange()
    {
        waterChange = 0;
        fireChange = 0;
        earthChange = 0;
        airChange = 0;
        Transform buildingParent = GameObject.Find("BuildingParent").transform;
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            if (buildingParent.GetChild(i).GetComponent<Machine>() != null)
            {
                Machine machine = buildingParent.GetChild(i).GetComponent<Machine>();
                waterChange += machine.waterChange;
                fireChange += machine.fireChange;
                earthChange += machine.earthChange;
                airChange += machine.airChange;
            }
        }
    }
}
