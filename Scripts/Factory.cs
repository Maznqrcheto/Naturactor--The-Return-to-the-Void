using UnityEngine;

public class Factory
{
    public Sprite Sprite { get; set; }
    public int Type { get; set; }

    //These two start their count from the bottom left corner of the sprite
    public Vector2 Input { get; set; }
    public Vector2 Output { get; set; }

    public Factory(Sprite sprite, Vector2 input, Vector2 output, int type)
    {
        Sprite = sprite;
        Input = input;
        Output = output;
        Type = type;
    }
    public Factory(Sprite sprite, int type)
    {
        Sprite = sprite;
        Type = type;
    }
}
