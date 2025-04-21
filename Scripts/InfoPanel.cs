using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InfoPanel : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;
    public GameObject Panel;
    public Text coordinates;
    public Text objectName;
    public Text description;
    public Image icon;

    GameObject selectedGameObject;
    private void Awake()
    {
        coordinates.text = string.Empty;
        objectName.text = string.Empty;
        description.text = string.Empty;
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

            //Check structure grid first
            if (genMap.structureGrid[(int)mousePosition.x, (int)mousePosition.y] != null)
            {
                Structure currentStructure = genMap.structureGrid[(int)mousePosition.x, (int)mousePosition.y].GetComponent<Structure>();
                selectedGameObject = currentStructure.gameObject;

                coordinates.text = $"({currentStructure.position.x}, {currentStructure.position.y})";
                objectName.text = currentStructure.GetName();
                description.text = currentStructure.GetDescription();
                icon.sprite = currentStructure.gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else if (genMap.grid[(int)mousePosition.x, (int)mousePosition.y] != null)
            {
                Tile currentTile = genMap.grid[(int)mousePosition.x, (int)mousePosition.y].GetComponent<Tile>();
                selectedGameObject = currentTile.gameObject;

                coordinates.text = $"({currentTile.gameObject.transform.position.x}, {currentTile.gameObject.transform.position.y})";
                objectName.text = currentTile.GetName();
                description.text = currentTile.GetDescription();
                icon.sprite = currentTile.gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            Panel.SetActive(true);

            //Add highlight to selected object
            Color objectColor = selectedGameObject.GetComponent<SpriteRenderer>().color;
            objectColor.r = 0.5f;
            objectColor.g = 0.5f;
            objectColor.b = 0.5f;
            selectedGameObject.GetComponent<SpriteRenderer>().color = objectColor;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && selectedGameObject != null)
        {
            //Remove the highlight
            Color objectColor = selectedGameObject.GetComponent<SpriteRenderer>().color;
            objectColor.r = 1f;
            objectColor.g = 1f;
            objectColor.b = 1f;
            selectedGameObject.GetComponent<SpriteRenderer>().color = objectColor;

            selectedGameObject = null;
            Panel.SetActive(false);
        }
    }
}