using UnityEngine;
public class ElementalDisbalanceEventManager : MonoBehaviour, IEventTickManager
{
    public EventManagerComponents components;
    public Flood floodEvent;
    public Drought droughtEvent;
    public Fire fireEvent;

    public void SetMapValuesForEvent(MapValues eventInstance)
    {
        eventInstance.mapGenerator = components.mapGenerator;
        eventInstance.grid = components.grid;
    }

    public void CheckEvents(ulong tick)
    {
        float fireLevel = components.fireLevel;
        float waterLevel = components.waterLevel;
        float airLevel = components.airLevel;
        float earthLevel = components.earthLevel;

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