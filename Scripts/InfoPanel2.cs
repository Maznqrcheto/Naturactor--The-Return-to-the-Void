using UnityEngine;
using UnityEngine.UI;
public class InfoPanel2 : MonoBehaviour
{
    public PlaceMachine actionManager;
    public GameObject panel;

    public Text nameText;

    public Text waterText;
    public Text fireText;
    public Text earthText;
    public Text airText;

    public Text descriptionText;

    public Image icon;

    private void Update()
    {
        if(actionManager.selectedfactory == -1 && actionManager.isChoppingTrees == false)
            panel.SetActive(false);
        else
            panel.SetActive(true);
    }
    public void CurrentAction(int selectedAction)
    {
        switch(selectedAction)
        {
            case 0:
                nameText.text = "Chop Trees";
                waterText = ColourText(waterText, 0.0f);
                fireText = ColourText(fireText, 0.5f);
                earthText = ColourText(earthText, -0.5f);
                airText = ColourText(airText, 0.0f);
                //descriptionText.text = "Chop Trees";
                break;
            default:
                Factory curSelection = actionManager.factoryTypes[selectedAction - 1];
                switch(selectedAction)
                {
                    case 1:
                        nameText.text = "Drill";
                        break;
                    case 2:
                        nameText.text = "Generator";
                        break;
                    case 3:
                        nameText.text = "Conveyor belt";
                        break;
                    case 4:
                        nameText.text = "Container";
                        break;
                }
                waterText = ColourText(waterText, curSelection.waterChange);
                fireText = ColourText(fireText, curSelection.fireChange);
                earthText = ColourText(earthText, curSelection.earthChange);
                airText = ColourText(airText, curSelection.airChange);
                //descriptionText.text = curSelection.description;
                icon.sprite = curSelection.Sprite;
                break;
        }
    }
    public Text ColourText(Text text, float value)
    {
        if (value == 0)
            text.color = Color.gray;
        else if (value > 0)
            text.color = Color.green;
        else 
            text.color = Color.red;

        return text;
    }
}
