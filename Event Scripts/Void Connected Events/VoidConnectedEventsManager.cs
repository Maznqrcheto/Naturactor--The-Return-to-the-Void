using UnityEngine;

public class VoidConnectedEventsManager : MonoBehaviour, IEventTickManager
{
    public EventManagerComponents components;
    public Doppelganger doppelgangerEvent;

    public void SetMapValuesForEvent(MapValues eventInstance)
    {
        eventInstance.mapGenerator = components.mapGenerator;
        eventInstance.grid = components.grid;
    }

    public void CheckEvents(ulong tick)
    {
        // if (doppelgangerEvent.CanSpawnDoppelganger(tick))
        // {
        //     doppelgangerEvent.SpawnDoppelganger();
        // }
    }

    void Start()
    {
        doppelgangerEvent.Initialize();
        SetMapValuesForEvent(doppelgangerEvent);
    }
}
// public class MapValues : MonoBehaviour
// {
//     public GenerateMap mapGenerator;
//     public GameObject[,] grid;
//     public GameObject[,] structureGrid;
//     public Sprites spritesGetter;
// }