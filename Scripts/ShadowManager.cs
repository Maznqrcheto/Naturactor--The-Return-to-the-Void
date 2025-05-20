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
        CreateShadowForTrees();
    }
    public void CreateShadowForTrees()
    {

        for(int i = 0; i < genMap.x; i++)
        {
            for(int j = 0; j < genMap.y; j++)
            {
                GameObject currentTree = structrureGrid[i, j];

                if(currentTree != null)
                {
                    GameObject treeShadow = new GameObject(currentTree.name);

                    treeShadow.AddComponent<SpriteRenderer>();
                    treeShadow.GetComponent<SpriteRenderer>().sprite = currentTree.GetComponent<SpriteRenderer>().sprite;
                    treeShadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.4f);
                    treeShadow.GetComponent<SpriteRenderer>().sortingOrder = 1;

                    treeShadow.transform.position = new Vector2(currentTree.transform.position.x, currentTree.transform.position.y - 1);
                    treeShadow.transform.localScale = new Vector3(1, -0.4f, 1);

                    currentTree.GetComponent<Structure>().associatedShadow = treeShadow;
                }
                
            }
            
        }
    }
}
