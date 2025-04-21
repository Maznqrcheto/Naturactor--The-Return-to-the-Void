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

    private void Update()
    {
        UpdateItemCount();
        woodCountText.text = woodCount.ToString();
        coalCountText.text = coalCount.ToString();
        ironCountText.text = ironCount.ToString();
        copperCountText.text = copperCount.ToString();
    }

    public void UpdateItemCount()
    {
        Transform buildingParent = GameObject.Find("BuildingParent").transform;
        woodCount = 0;
        coalCount = 0;
        ironCount = 0;
        copperCount = 0;
        for(int i = 0; i < buildingParent.childCount; i++)
        {
            if(buildingParent.GetChild(i).GetComponent<Machine>() != null)
            {
                Machine currentMachine = buildingParent.GetChild(i).GetComponent<Machine>();
                if (currentMachine.type == 4)
                {
                    //Count items in container
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
