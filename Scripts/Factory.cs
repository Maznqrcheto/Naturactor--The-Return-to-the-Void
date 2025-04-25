using UnityEngine;

public class Factory
{
    public Sprite Sprite { get; set; }
    public int Type { get; set; }
    public string description;

    //These two start their count from the bottom left corner of the sprite
    public Vector2 Input { get; set; }
    public Vector2 Output { get; set; }

    public float waterChange;
    public float fireChange;
    public float earthChange;
    public float airChange;

    public float energyConsumption = -1;
    public Factory(Sprite sprite, Vector2 input, Vector2 output, int type)
    {
        Sprite = sprite;
        Input = input;
        Output = output;
        Type = type;
    }
    public Factory(Sprite sprite, Vector2 input, Vector2 output, int type, float waterChange, float fireChange, float earthChange, float airChange)
    {
        Input = input;
        Output = output;
        Sprite = sprite;
        Type = type;
        this.waterChange = waterChange;
        this.fireChange = fireChange;
        this.earthChange = earthChange;
        this.airChange = airChange;
    }

    public Factory(Sprite sprite, int type)
    {
        Sprite = sprite;
        Type = type;
    }
}
