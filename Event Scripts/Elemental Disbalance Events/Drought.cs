using UnityEngine;

public class Drought : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public bool droughtIsActive = false;
    public bool droughtOccured = false;

    public void Drought()
    {
        droughtIsActive = true;
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
            GameObject currentTile = new GameObject();
            currentTile.transform.parent = mapParent.transform;
            currentTile.transform.position = new Vector2(i, j);

            currentTile.AddComponent<SpriteRenderer>();
            currentTile.AddComponent<Tile>();
            currentTile.name = $"{i},{j}";
            grid[x+1, y] = tileOnLeft;
            grid[x-1, y] = tileOnRight;
            grid[x, y+1] = tileOnTop;
            grid[x, y-1] = tileOnBottom;

            if(tileOnLeft.GetComponent<Tile>().type == 0)
            {
                tileOnLeft.GetComponent<Tile>().type = Random.Range(1, 4);
                tileOnLeft.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
            }
            if( tileOnRight.GetComponent<Tile>().type == 0)
            {
                tileOnRight.GetComponent<Tile>().type = Random.Range(1, 4);
                tileOnRight.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
            }
            if(tileOnTop.GetComponent<Tile>().type == 0)
            {
                tileOnTop.GetComponent<Tile>().type = Random.Range(1, 4);
                tileOnTop.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
            }
            if(tileOnBottom.GetComponent<Tile>().type == 0)
            {              
                tileOnBottom.GetComponent<Tile>().type = Random.Range(1, 4);
                tileOnBottom.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
            }
            }
        }
        droughtOccured = true;
    }   
}