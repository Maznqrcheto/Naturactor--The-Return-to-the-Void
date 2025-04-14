using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;
    public List<Sprite> TileSprites = new List<Sprite>();
    void Start()
    {
        GenerateMapFromScratch();
    }
    public void GenerateMapFromScratch()
    {
        GameObject mapParent = new GameObject();
        mapParent.name = "TileParent";

        //Generate all tiles
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                GameObject currentTile = new GameObject();
                currentTile.transform.parent = mapParent.transform;
                currentTile.transform.position = new Vector2(i, j);

                currentTile.AddComponent<SpriteRenderer>();
                currentTile.AddComponent<Tile>();
                currentTile.name = $"{i},{j}";

                currentTile.GetComponent<Tile>().type = 1;
                currentTile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
            }
        }
        //Make Lakes

    }
    
}
