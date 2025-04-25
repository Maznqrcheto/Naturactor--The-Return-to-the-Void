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
    public string Name()
    {
        switch(type)
        {
            case 0:
                return "Coal";
            case 1:
                return "Iron Ore";
            case 2:
                return "Copper Ore";
            case 3:
                return "Wood";
            case 4:
                return "Iron Ingot";
            case 5:
                return "Copper Ingot";
            case 6:
                return "Tools";
            case 7:
                return "Advanced Tools";
            case 8:
                return "Wooden Biomass";
        }
        return "invalid item";
    }
}
