using UnityEngine;

public class Tile : MonoBehaviour
{
    //0 - water
    //1 - grass
    //2 - coal
    //3 - iron
    public int type = 0;

    //For veins like coal and iron
    public int material;

    public string GetName()
    {
        switch (type)
        {
            case 0: return "water";
            case 1: return "grass";
            case 2: return "coal";
            case 3: return "iron";
        }
        return "No Name";
    }
    public string GetDescription()
    {
        switch (type)
        {
            case 0:
                return "Just water.";
            case 1:
                return "An ordinary grass patch.";
            case 2:
                return "Rock filled with coal to the brim. Seems like it is begging to be extracted from the ground.";
            case 3:
                return "An iron patch. Useful for making machines and producing materials.";
        }

        return "No Description";
    }
}
