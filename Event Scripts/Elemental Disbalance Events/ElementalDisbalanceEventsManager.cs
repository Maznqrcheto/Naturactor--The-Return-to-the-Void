using UnityEngine;
public class ElementalDisbalanceEventManager : EventManagerComponents, IEventTickManager
{
    public Flood floodEvent;
    public Drought droughtEvent;
    public Fire fireEvent;

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
        bool fireCanOccur = (fireLevel - airLevel > 20f && !fireEvent.fireOccured && fireEvent.fireCooldown == 2400);

        if (droughtCanOccur) // Drought Event
        {
            droughtEvent.Initialize();
            droughtEvent.StartDrought();
        }
        if (floodCanOccur) // Flood Event
        {
            floodEvent.Initialize();
            floodEvent.StartFlood();
        }
        if (fireCanOccur) // Fire Event
        {
            fireEvent.Initialize();
            fireEvent.StartFire();
        }
    }
    void Start()
    {
        SetMapValuesForEvent(droughtEvent);
        SetMapValuesForEvent(floodEvent);
        SetMapValuesForEvent(fireEvent);
    }
}