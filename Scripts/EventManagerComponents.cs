using UnityEngine;

public class EventManagerComponents : MonoBehaviour
{
    public ElementalDisbalanceEventManager elementalDisbalanceEventManager;
    [Header("Slide bars")]
    [SerializeField] ProgressBarController fireBar;
    [SerializeField] ProgressBarController waterBar;
    [SerializeField] ProgressBarController airBar;
    [SerializeField] ProgressBarController earthBar;
    [SerializeField] ProgressBarController happinessBar;

    [Header("Element levels")]
    public float fireLevel;
    public float waterLevel;
    public float airLevel;
    public float earthLevel;

    [Header("Happiness level")]
    [SerializeField] float happinessLevel;

    [Header("Element changes")]
    [SerializeField] float fireChange;
    [SerializeField] float waterChange;
    [SerializeField] float airChange;
    [SerializeField] float earthChange;

    [Header("Happiness change")]
    [SerializeField] float happinessChange;

    [Header("Tick system")]
    [SerializeField] ulong ticksToChange = 0;
    [SerializeField] TickSystem tickSystem;

    public GenerateMap mapGenerator { get; set; }
    public GameObject[,] grid { get; set; }


    public void Update()
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
    public void SetElementalProgress(float value)
    {
        fireBar.SetProgress(value);
        waterBar.SetProgress(value);
        airBar.SetProgress(value);
        earthBar.SetProgress(value);
        happinessBar.SetProgress(value);
    }
    public void GetElementalProgress()
    {
        fireLevel = fireBar.GetProgress();
        waterLevel = waterBar.GetProgress();
        airLevel = airBar.GetProgress();
        earthLevel = earthBar.GetProgress();
        happinessLevel = happinessBar.GetProgress();
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
    public void SetMapValuesForEvent(IEventMapGetValues currentEvent)
    {
        currentEvent.mapGenerator = mapGenerator;
        currentEvent.grid = mapGenerator.grid;
    }
    public interface IEventMapGetValues
{
    public GenerateMap mapGenerator { get; set; }
    public GameObject[,] grid { get; set; }
}
}
public interface IEventTickManager
{
    public void CheckEvents(ulong tick);
}
