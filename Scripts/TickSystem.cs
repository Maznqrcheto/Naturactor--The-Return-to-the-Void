using System.Collections;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public float tickLength;
    public ulong tickTime = 1;
    private void Start()
    {
        StartCoroutine(Tick());
    }
    IEnumerator Tick()
    {
        while (true)
        {
            tickTime++;
            yield return new WaitForSeconds(tickLength);
        }
    }
}
