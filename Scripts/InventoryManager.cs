using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
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

    public int toolsCount;
    public Text toolsCountText; 

    public int advToolsCount;
    public Text advToolsCountText; 

    public int biomassCount;
    public Text biomassCountText;
    private void Update()
    {
        UpdateItemCount();
        woodCountText.text = woodCount.ToString();
        coalCountText.text = coalCount.ToString();
        ironCountText.text = ironCount.ToString();
        copperCountText.text = copperCount.ToString();
        ironBarCountText.text = ironBarCount.ToString();
        copperBarCountText.text = copperBarCount.ToString();
        toolsCountText.text = toolsCount.ToString();
        advToolsCountText.text = advToolsCount.ToString();
        biomassCountText.text = biomassCount.ToString();
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
        toolsCount = 0;
        advToolsCount = 0;
        biomassCount = 0;

        for (int i = 0; i < buildingParent.childCount; i++)
        {
            if (buildingParent.GetChild(i).GetComponent<Machine>() != null)
            {
                Machine currentMachine = buildingParent.GetChild(i).GetComponent<Machine>();
                if (currentMachine.type == 4 && currentMachine.inventory.Count > 0)
                {
                    Item topItem = (Item)currentMachine.inventory.Peek();

                    switch (topItem.type)
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
                        case 6:
                            toolsCount += currentMachine.inventory.Count;
                            break;
                        case 7:
                            advToolsCount += currentMachine.inventory.Count;
                            break;
                        case 8:
                            biomassCount += currentMachine.inventory.Count;
                            break;
                    }
                }
                if (currentMachine.type == -1)
                {
                    if (currentMachine.inventory.Count > 0)
                        woodCount += currentMachine.inventory.Count;
                }
            }
        }
    }
    public void RemoveItem(int[] itemTypesToRemove, int[] countOfItemsToRemove)
    {
        Transform buildingParent = GameObject.Find("BuildingParent").transform;

        for (int currentItem = 0; currentItem < itemTypesToRemove.Length; currentItem++)
        {
            int countToRemove = countOfItemsToRemove[currentItem];
            for (int objectToCheck = 0; objectToCheck < buildingParent.childCount; objectToCheck++)
            {
                Machine currentMachine = buildingParent.GetChild(objectToCheck).GetComponent<Machine>();
                if ((currentMachine.type == 4 || currentMachine.type == -1) && currentMachine.inventory.Count > 0)
                {
                    Item containerItem = (Item)currentMachine.inventory.Peek();
                    if (containerItem.type == itemTypesToRemove[currentItem])
                    {
                        if (currentMachine.inventory.Count < countToRemove)
                        {
                            countToRemove -= currentMachine.inventory.Count;
                            currentMachine.inventory = new Stack();
                        }
                        else
                        {
                            for (int k = 0; k < countToRemove; k++)
                                currentMachine.inventory.Pop();
                            return;
                        }
                    }
                }

            }
        }
    }
}
