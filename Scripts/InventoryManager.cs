using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class InventoryManager : MonoBehaviour
{
    public int woodCount;
    public Text woodCountText;

    public int coalCount;
    public Text coalCountText;

    public int ironCount;
    public Text ironCountText;

    public int copperCount;
    public Text copperCountText;

    public int ironBarCount;
    public Text ironBarCountText;

    public int copperBarCount;
    public Text copperBarCountText;

    private void Update()
    {
        UpdateItemCount();
        woodCountText.text = woodCount.ToString();
        coalCountText.text = coalCount.ToString();
        ironCountText.text = ironCount.ToString();
        copperCountText.text = copperCount.ToString();
        ironBarCountText.text = ironBarCount.ToString();
        copperBarCountText.text = copperBarCount.ToString();
    }

    public void UpdateItemCount()
    {
        Transform buildingParent = GameObject.Find("BuildingParent").transform;
        woodCount = 0;
        coalCount = 0;
        ironCount = 0;
        copperCount = 0;
        ironBarCount = 0;
        copperBarCount = 0;
        for(int i = 0; i < buildingParent.childCount; i++)
        {
            if(buildingParent.GetChild(i).GetComponent<Machine>() != null)
            {
                Machine currentMachine = buildingParent.GetChild(i).GetComponent<Machine>();
                if (currentMachine.type == 4 && currentMachine.inventory.Count > 0)
                {
                    Item topItem = (Item)currentMachine.inventory.Peek();

                    switch(topItem.type)
                    {
                        case 0:
                            coalCount += currentMachine.inventory.Count;
                            break;
                        case 1:
                            ironCount += currentMachine.inventory.Count;
                            break;
                        case 2:
                            copperCount += currentMachine.inventory.Count;
                            break;
                        case 3:
                            woodCount += currentMachine.inventory.Count;
                            break;
                        case 4:
                            ironBarCount += currentMachine.inventory.Count;
                            break;
                        case 5:
                            copperBarCount += currentMachine.inventory.Count;
                            break;
                    }
                }
                if(currentMachine.type == -1)
                {
                    if(currentMachine.inventory.Count > 0)
                        woodCount += currentMachine.inventory.Count;
                }
            }
        } 
    }
}
