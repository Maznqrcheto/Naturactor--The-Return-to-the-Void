using UnityEngine;

public class Structure : MonoBehaviour
{
    //0 - forest
    //1 - reactor
    //2 - harvesting factory
    //3 - refining factory
    //4 - transportation
    //5 - Volcanos
    public int type = 0;
    public Vector2 position;

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
