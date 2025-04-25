using UnityEngine;

public class Item
{
    //0 - Coal
    //1 - Iron
    //2 - Copper
    //3 - Wood
    //4 - Iron Ingot
    //5 - Copper Ingot
    //6 - Tools
    //7 - Advanced Tools
    //8 - Wooden biomass
    public int type;

    public Item(int type)
    {
        this.type = type;
    }
}
