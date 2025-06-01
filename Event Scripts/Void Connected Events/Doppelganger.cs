using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Doppelganger : MapValues
{
    public PlaceMachine placeMachine;
    public List<Sprite> GrassSprites;
    public List<Sprite> GrassCliffSprites;
    public List<Sprite> TreeSprites;
    public List<Sprite> DoppelgangerSprites;
    float spawnChance = 0f;
    public bool canSpawnDoppelganger = true; // this needs to be set to false ! for test purposes, it's set to true. This variable is dependent on value of Inspiration!
    public bool doppelgangerIsActive = false;
    public bool doppelgangerOccured = false;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int doppelgangerCooldown = 2400; //2400 = 10 minutes
    public int counterForSpawn = 0;
    public GameObject doppelganger;

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
        DoppelgangerSprites = spritesGetter.DoppelgangerSprites;
    }

    IEnumerator<object> TickDoppelganger()
    {
        while (true)
        {
            if (doppelgangerIsActive && counter == Random.Range(100, 120)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
            {
                RevertDoppelganger();
                counter = 1;
                yield break;
            }
            counter++;
            yield return new WaitForSeconds(counterTickLength);
        }
    }
    IEnumerator<object> TickDoppelgangerCooldown()
    {
        while (true)
        {
            if (doppelgangerOccured && doppelgangerCooldown > 0)
            {
                doppelgangerCooldown--;
            }
            else
            {
                doppelgangerOccured = false;
                doppelgangerIsActive = false;
                yield break;
            }
            yield return new WaitForSeconds(counterTickLength);
        }
    }

    public bool CanSpawnDoppelganger(ulong tick)
    {
        // if (canSpawnDoppelganger)
        // {
        //     if (counterForSpawn % 120 == 0) // every 30 seconds, because tickLength = 0.25 seconds
        //     {
        //         spawnChance += 0.5f;
        //     }
        //     counterForSpawn++;
        //     if (Random.Range(0f, 100f) < spawnChance)
        //     {
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }
        // else
        // {
        //     return false;
        // }
        return true;
    }
    public void SpawnDoppelganger()
    {
        Debug.Log("Doppelganger spawned!");
        doppelgangerIsActive = true;
        StartCoroutine(TickDoppelganger());

        // Randomly select a position on the map
        int x = Random.Range(0, mapGenerator.x);
        int y = Random.Range(0, mapGenerator.y);
        Vector2 doppelgangerPosition = new Vector2(x, y);

        while (!placeMachine.GetComponent<PlaceMachine>().CheckIfCanPlace(new Vector2(doppelgangerPosition.x - 5, doppelgangerPosition.y - 5), 12, 12))
        {
            doppelgangerPosition = new Vector2(Random.Range(0, mapGenerator.x), Random.Range(0, mapGenerator.y));
        }
        doppelganger = new GameObject("doppelganger");
        doppelganger.transform.parent = GameObject.Find("BuildingParent").transform;

        doppelganger.AddComponent<Machine>();
        doppelganger.GetComponent<Machine>().type = -1;
        doppelganger.GetComponent<Machine>().hasInput = false;
        doppelganger.GetComponent<Machine>().hasOutput = false;
        doppelganger.GetComponent<Machine>().UpdateInventorySize();

        doppelganger.AddComponent<SpriteRenderer>();
        doppelganger.GetComponent<SpriteRenderer>().sprite = DoppelgangerSprites[Random.Range(0, DoppelgangerSprites.Count)];

        doppelganger.AddComponent<Structure>();
        doppelganger.GetComponent<Structure>().type = 1;
        doppelganger.GetComponent<Structure>().position = new Vector2(doppelgangerPosition.x, doppelgangerPosition.y);

        doppelganger.transform.position = new Vector2(doppelgangerPosition.x + 0.5f, doppelgangerPosition.y + 0.5f);
        structureGrid[(int)doppelgangerPosition.x, (int)doppelgangerPosition.y] = doppelganger;
        structureGrid[(int)doppelgangerPosition.x + 1, (int)doppelgangerPosition.y] = doppelganger;
        structureGrid[(int)doppelgangerPosition.x + 1, (int)doppelgangerPosition.y + 1] = doppelganger;
        structureGrid[(int)doppelgangerPosition.x, (int)doppelgangerPosition.y + 1] = doppelganger;

        doppelgangerOccured = true;
        doppelganger.GetComponent<SpriteRenderer>().sortingOrder = 1000;
        // mapGenerator.UpdateSortingOrderForStructures();
    }

    public void EatHappiness(ulong tick)
    {
        if (doppelgangerIsActive)
        {
            
            components.happinessLevel -= 0.1f;
            Debug.Log("Doppelganger ate happiness!");
        }
    }
    void RevertDoppelganger()
    {
        Debug.Log("Doppelganger reverted1!");
        Destroy(doppelganger);

        Debug.Log("Doppelganger reverted2!");
        doppelgangerIsActive = false;
        spawnChance = 0f;
        StartCoroutine(TickDoppelgangerCooldown());
    }
}