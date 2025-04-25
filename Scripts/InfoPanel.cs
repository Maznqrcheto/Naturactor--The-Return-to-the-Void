using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class InfoPanel : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;
    public GameObject Panel;

    public Text objectName;
    public Image icon;
    public Dropdown inventory;

    GameObject selectedGameObject;
    private void Awake()
    {
        objectName.text = string.Empty;
        icon.sprite = null;
        Panel.SetActive(false);
    }
    void Update()
    {
        if ((pauseMenu != null && pauseMenu.isPaused) || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.x = Mathf.Round(mousePosition.x); mousePosition.y = Mathf.Round(mousePosition.y);

            //Remove previous highlight
            if (selectedGameObject != null)
            {
                Color previousObjectColor = selectedGameObject.GetComponent<SpriteRenderer>().color;
                previousObjectColor.r = 1f;
                previousObjectColor.g = 1f;
                previousObjectColor.b = 1f;
                selectedGameObject.GetComponent<SpriteRenderer>().color = previousObjectColor;
            }

            if (genMap.structureGrid[(int)mousePosition.x, (int)mousePosition.y] != null)
            {
                selectedGameObject = genMap.structureGrid[(int)mousePosition.x, (int)mousePosition.y];

                objectName.text = selectedGameObject.GetComponent<Structure>().GetName();
                icon.sprite = selectedGameObject.GetComponent<SpriteRenderer>().sprite;
                Panel.SetActive(true);

                Color objectColor = selectedGameObject.GetComponent<SpriteRenderer>().color;
                objectColor.r = 0.5f;
                objectColor.g = 0.5f;
                objectColor.b = 0.5f;
                selectedGameObject.GetComponent<SpriteRenderer>().color = objectColor;
            }
            else
            {
                if(selectedGameObject != null)
                {
                    Color objectColor = selectedGameObject.GetComponent<SpriteRenderer>().color;
                    objectColor.r = 1f;
                    objectColor.g = 1f;
                    objectColor.b = 1f;
                    selectedGameObject.GetComponent<SpriteRenderer>().color = objectColor;
                }
                
                selectedGameObject = null;
                Panel.SetActive(false);
            }
        }
        if (selectedGameObject != null && selectedGameObject.GetComponent<Machine>() != null)
        {
            Machine currentMachine = selectedGameObject.GetComponent<Machine>();
            if(currentMachine.type != 4 && currentMachine.inventorySize > 0)
            {
                List<string> itemsInInventory = new List<string>();
                foreach (Item item in currentMachine.inventory)
                {
                    itemsInInventory.Add(item.Name());
                }

                inventory.ClearOptions();
                inventory.AddOptions(itemsInInventory);
            }
        }
    }
}