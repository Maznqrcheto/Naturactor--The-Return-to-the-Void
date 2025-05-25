using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Doppelganger : MapValues
{
    public List<Sprite> GrassSprites;
    public List<Sprite> GrassCliffSprites;
    public List<Sprite> TreeSprites;
    public void Initialize()
    {
        SetMap();
        SetSpritesForUse(); 
    }

    void SetMap()
    {
        grid = mapGenerator.grid;
        structureGrid = mapGenerator.structureGrid;
    }
    void SetSpritesForUse()
    {
        GrassSprites = spritesGetter.GrassSprites;
        GrassCliffSprites = spritesGetter.GrassCliffSprites;
        TreeSprites = spritesGetter.TreeSprites;
    }
}