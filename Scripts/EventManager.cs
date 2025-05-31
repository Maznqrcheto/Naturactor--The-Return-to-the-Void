using UnityEngine;

public class EventManager : MonoBehaviour, IEventTickManager
{
    public ElementalDisbalanceEventManager elementalDisbalanceEventManager;
    public VoidConnectedEventsManager voidConnectedEventsManager;
    public EventManagerComponents components;

    public float fireLevel
    {
        get => components.fireLevel;
        set
        {
            components.fireLevel = value;
            components.fireBar.SetProgress(value);
        }
    }
    public float waterLevel
    {
        get => components.waterLevel;
        set
        {
            components.waterLevel = value;
            components.waterBar.SetProgress(value);
        }
    }
    public float airLevel
    {
        get => components.airLevel;
        set
        {
            components.airLevel = value;
            components.airBar.SetProgress(value);
        }
    }
    public float earthLevel
    {
        get => components.earthLevel;
        set
        {
            components.earthLevel = value;
            components.earthBar.SetProgress(value);
        }
    }
    public float happinessLevel
    {
        get => components.happinessLevel;
        set
        {
            components.happinessLevel = value;
            components.happinessBar.SetProgress(value);
        }
    }
    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        elementalDisbalanceEventManager.CheckEvents(tick);
        voidConnectedEventsManager.CheckEvents(tick);

    }
    void Start()
    {
        components.SetElementalProgress(100f);
        components.GetElementalProgress();
    }
}
public class MapValues : MonoBehaviour
    {
        public GenerateMap mapGenerator;
        public GameObject[,] grid;
        public GameObject[,] structureGrid;
        public Sprites spritesGetter;
    }