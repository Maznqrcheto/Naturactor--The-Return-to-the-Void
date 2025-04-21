using System.Collections;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public float tickLength;
    public ulong tickTime = 1;
    public EventManager eventManager;
    private void Start()
    {
        StartCoroutine(Tick());
    }
    IEnumerator Tick()
    {
        while (true)
        {
            tickTime++;
            if(eventManager != null)
            {
                eventManager.CheckEvents(tickTime);
            }
            yield return new WaitForSeconds(tickLength);
        }
    }
}
