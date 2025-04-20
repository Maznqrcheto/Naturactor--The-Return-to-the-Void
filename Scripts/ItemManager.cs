using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Sprite> itemSprites;
    Transform itemParent;
    GenerateMap genMap;
    private void Start()
    {
        itemParent = new GameObject("ItemParent").transform;
        genMap = GetComponent<GenerateMap>();
    }
    public GameObject CreateItemEntity(Item item, GameObject gameObjToAttach)
    {
        GameObject itemEntity = new GameObject("item");
        itemEntity.transform.parent = itemParent;

        itemEntity.AddComponent<SpriteRenderer>();
        itemEntity.GetComponent<SpriteRenderer>().sprite = itemSprites[item.type];
        itemEntity.GetComponent<SpriteRenderer>().sortingOrder = genMap.y + 100;

        itemEntity.transform.localScale = new Vector2(0.5f, 0.5f);

        itemEntity.AddComponent<ItemEntity>();
        itemEntity.GetComponent<ItemEntity>().boundGameObject = gameObjToAttach;
        itemEntity.GetComponent<ItemEntity>().item = item;

        return itemEntity;
    }
}
