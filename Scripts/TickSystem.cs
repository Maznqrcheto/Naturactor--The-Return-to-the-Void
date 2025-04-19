using System.Collections;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public float tickLength;
    private void Start()
    {
        StartCoroutine(Tick());
    }
    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickLength);
        }
    }
}
