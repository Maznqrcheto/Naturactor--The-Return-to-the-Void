using UnityEngine;
public class ElementalDisbalanceEventManager : EventManagerComponents, IEventTickManager
{
    [SerializeField] private Flood floodEvent;
    [SerializeField] private Drought droughtEvent;

    public void SetMapValuesForEvent(MapValues eventInstance)
    {
        eventInstance.mapGenerator = mapGenerator;
        eventInstance.grid = grid;
    }

    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {

        GetElementalProgress();

        bool droughtCanOccur = (fireLevel - waterLevel > 20f && !droughtEvent.droughtOccured && droughtEvent.droughtCooldown == 2400);
        bool floodCanOccur = (waterLevel - fireLevel > 20f && !floodEvent.floodOccured && floodEvent.floodCooldown == 2400);

        if (droughtCanOccur) // Drought Event
        {
            droughtEvent.StartDrought();
        }
        if (floodCanOccur) // Flood Event
        {
            floodEvent.StartFlood();
        }
    }
    void Start()
    {
        SetMapValuesForEvent(droughtEvent);
        SetMapValuesForEvent(floodEvent);
        floodEvent.Initialize();
        droughtEvent.Initialize();
    }
}
public class MapValues : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
}