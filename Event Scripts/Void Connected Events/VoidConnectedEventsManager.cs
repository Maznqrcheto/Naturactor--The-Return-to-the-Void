using UnityEngine;

public class VoidConnectedEventsManager : EventManagerComponents, IEventTickManager
{
    public Doppelganger doppelgangerEvent;

    public void SetMapValuesForEvent(MapValues eventInstance)
    {
        eventInstance.mapGenerator = mapGenerator;
        eventInstance.grid = grid;
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