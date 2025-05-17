using UnityEngine;
public class ElementalDisbalanceEventManager : EventManagerComponents, IEventTickManager, EventManagerComponents.IEventMapGetValues
{
    [SerializeField] private Flood floodEvent;
    [SerializeField] private Drought droughtEvent;

    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {

        GetElementalProgress();

        bool droughtCanOccur = (fireLevel - waterLevel > 20f && !droughtEvent.droughtOccured && droughtEvent.droughtCooldown == 2400);
        bool floodCanOccur = (waterLevel - fireLevel > 20f && !floodEvent.floodOccured && floodEvent.floodCooldown == 2400);

        if (droughtCanOccur) // Drought Event
        {
            SetMapValuesForEvent(droughtEvent);
            droughtEvent.StartDrought();
        }
        if (floodCanOccur) // Flood Event
        {
            SetMapValuesForEvent(floodEvent);
            floodEvent.StartFlood();
        }
    }
    void Start()
    {
        
    }
}