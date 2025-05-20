using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    [SerializeField] GenerateMap genMap;
    GameObject[,] structrureGrid;

    private void Start()
    {
        structrureGrid = genMap.structureGrid;
    }
    public void CreateShadowForEveryObject()
    {
        structrureGrid = genMap.structureGrid;

        for (int i = 0; i < genMap.x; i++)
        {
            for(int j = 0; j < genMap.y; j++)
            {
                GameObject currentObject = structrureGrid[i, j];

                if(currentObject != null)
                {
                    GameObject objectShadow = new GameObject(currentObject.name);

                    objectShadow.AddComponent<SpriteRenderer>();
                    objectShadow.GetComponent<SpriteRenderer>().sprite = currentObject.GetComponent<SpriteRenderer>().sprite;
                    objectShadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.4f);
                    objectShadow.GetComponent<SpriteRenderer>().sortingOrder = 1;

                    Vector2 shadowPosition = currentObject.transform.position;
                    shadowPosition.y = shadowPosition.y - (currentObject.GetComponent<SpriteRenderer>().sprite.texture.height / 32) / 1.8f;

                    objectShadow.transform.position = shadowPosition;
                    objectShadow.transform.localScale = new Vector3(1, -0.3f, 1);

                    currentObject.GetComponent<Structure>().associatedShadow = objectShadow;
                }
                
            }
            
        }
    }
}
