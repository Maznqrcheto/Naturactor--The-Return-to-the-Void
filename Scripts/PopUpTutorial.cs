using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopUpTutorial : MonoBehaviour
{
    public GameObject popUpPanel;
    public Text tutorialText;
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;
    private List<string> tutorialMessages;
    private int currentPageIndex = 0;

    void Awake()
    {
        tutorialMessages = new List<string>
        {
            "Hello dear traveler, my name is Naturactor! In this tutorial, I will guide you through the basics of the gameplay.",
            "Your goal is to manage the elements of nature and keep the balance in the world. You will be able to build various structures and interact with the environment.",
            "To start, let's take a look at the map. You can move by using W,A,S,D on your keyboard. You can zoom in and out using the mouse wheel.",
            "Now, let's build your first structure. Take a look at the building menu on the bottom of the screen. You can select different buildings to construct.",
            "To build your first structure, you'll need to collect some wood first. Press on the Chop Trees button and chop some trees down.",
            "You wanna be careful, because every action you take disrupts the elemental balance. You wanna keep every element balanced to avoid disasters.",
            "After you collect the wood, select the drill building and place it on top of a resource like copper or iron.",
            "Placing resources in containers allows you to add them to your stockpile. You can use those stockpiled resources to build other structures.",
            "These resources are essential for building and upgrading structures. Make sure to keep an eye on them and manage them wisely.",
            "Now, to the interesting part! You can interact with the environment and change the elements. For example, you can craft structures and affect fire, water, earth, and air and disbalance them. Keep an eye on that! Disasters may not be averted!.",
            "Oh, oh! I almost forgot! My dream is to find the once lost element of nature - the Void! Do you want to hear more about it?",
            "Long ago, when heavens of the reactors were created, the Void was a part of the balance. But one day, it was lost in the depths of the universe.",
            "I believe that if we work together, we can find the Void and restore the full balance of nature and restore my once lost strength! I will help you with that! I will be your guide and assistant in this journey."
        };
    }

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopUp);
        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);
        UpdatePage();
    }

    public void UpdatePage()
    {
        tutorialText.text = tutorialMessages[currentPageIndex];
        previousButton.gameObject.SetActive(currentPageIndex > 0);
        nextButton.gameObject.SetActive(currentPageIndex < tutorialMessages.Count - 1);
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialMessages.Count - 1)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }

    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
    }
}
