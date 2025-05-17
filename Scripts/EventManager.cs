using UnityEngine;

public class EventManager : EventManagerComponents, IEventTickManager
{
    public void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        // elementalDisbalanceEventManager.CheckEvents(tick);
        GetElementalProgress();
    }
    void Start()
    {
        SetElementalProgress(100f);
    }
}