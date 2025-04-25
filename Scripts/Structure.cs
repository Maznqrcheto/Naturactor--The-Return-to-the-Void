using UnityEngine;

public class Structure : MonoBehaviour
{
    //0 - tree
    //1 - reactor
    //2 - harvesting factory
    //3 - refining factory
    //4 - transportation
    //5 - Volcanos
    public int type = 0;
    public Vector2 position;
    public Vector2 originPosition;
    private void Start()
    {
        Texture spriteTexture = GetComponent<SpriteRenderer>().sprite.texture;

        //Calculate position of bottom left corner
        if((spriteTexture.width / 32) % 2 == 0) 
            originPosition.x = position.x - (spriteTexture.width / 32) / 2 + 0.5f;
        else
            originPosition.x = position.x - (spriteTexture.width / 32) / 2;

        if((spriteTexture.height / 32) % 2 == 0)
            originPosition.y = position.y - (spriteTexture.height / 32) / 2 + 0.5f;
        else
            originPosition.y = position.y - (spriteTexture.height / 32) / 2;
    }
    public string GetName()
    {
        switch (type)
        {
            case 0: return "tree";
            case 1: return "the naturactor";
            case 2: return "drill";
            case 3: return "refinery";
            case 4: return "transport";
            case 5: return "volcano";
        }
        return "No Name";
    }
    public string GetDescription()
    {
        switch (type)
        {
            case 0:
                return "An ordinary tree.";
            case 1:
                return "The Naturactor. The essence of all life.";
            case 2:
                return "A drill for harvesting resources from the ground.";
            case 3:
                return "Useful for refining materials. That's basically it.";
            case 4:
                return "Used for transporting materials";
            case 5:
                return "Better watch out";
        }

        return "No Description";
    }
}
