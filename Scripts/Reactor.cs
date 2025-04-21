using UnityEngine;
using UnityEngine.UI;
public class Reactor : MonoBehaviour
{
    [Header("Sliders")]
    public ProgressBarController WaterSlider;
    public ProgressBarController FireSlider;
    public ProgressBarController EarthSlider;
    public ProgressBarController AirSlider;

    [Header("Element values")]
    public float waterElement;
    public float fireElement;
    public float earthElement;
    public float airElement;

    [Header("Element Change")]
    public float waterChange;
    public float fireChange;
    public float earthChange;
    public float airChange;

    public ulong ticksToChange = 0;
    public TickSystem tickSystem;
    private void Update()
    {
        if (ticksToChange == 0)
            ticksToChange = tickSystem.tickTime + 30;

        if(ticksToChange <= tickSystem.tickTime)
        {
            waterElement += waterChange;
            fireElement += fireChange;
            earthElement += earthChange;
            airElement += airChange;
            ticksToChange = 0;
        }

        WaterSlider.SetProgress(waterElement);
        FireSlider.SetProgress(fireElement);
        EarthSlider.SetProgress(earthElement);
        AirSlider.SetProgress(airElement);
    }
}
