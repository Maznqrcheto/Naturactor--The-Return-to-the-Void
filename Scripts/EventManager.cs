using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Slide bars")]
    public ProgressBarController fireBar;
    public ProgressBarController waterBar;
    public ProgressBarController airBar;
    public ProgressBarController earthBar;

    [Header("Element levels")]
    public float fireLevel;
    public float waterLevel;
    public float airLevel;
    public float earthLevel;

    [Header("Element changes")]
    public float fireChange;
    public float waterChange;
    public float airChange;
    public float earthChange;

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
            ticksToChange = 0;
        }

        waterBar.SetProgress(waterLevel);
        fireBar.SetProgress(fireLevel);
        earthBar.SetProgress(earthLevel);
        airBar.SetProgress(airLevel);
    }
    public void GetElementProgress()
    {
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();
        airLevel = airBar.GetProgress();
        earthLevel = earthBar.GetProgress();
    }
    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        GetElementProgress();

        if (Mathf.Abs(fireLevel - waterLevel) > 20f)
        {
            //drought occurs
            Debug.Log("vagina");
            droughtEvent.mapGenerator = mapGenerator;
            droughtEvent.grid = mapGenerator.grid;
            droughtEvent.StartDrought();
            Debug.Log("Drought event triggered!");
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
