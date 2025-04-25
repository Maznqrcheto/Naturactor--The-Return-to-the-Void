using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopUpTutorial : MonoBehaviour
{
    public GameObject popUpPanel;
    public Button closeButton;
    public Button nextButton;
    public Button previousButton;
    public Text tutorialText;
    public List<string> tutorialMessages;
    public List<GameObject> tutorialPages;
    private int currentPageIndex = 0;

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopUp);
        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);
        firstPage();
        secondPage();
        thirdPage();
        fourthPage();
        fifthPage();
        sixthPage();
        seventhPage();
        eighthPage();
        ninthPage();
        tenthPage();
        eleventhPage();
        twelfthPage();
        UpdatePageVisibility();
    }

    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialPages.Count - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
        }
    }

    public void UpdatePageVisibility()
    {
        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == currentPageIndex);
        }
        previousButton.gameObject.SetActive(currentPageIndex > 0);
        nextButton.gameObject.SetActive(currentPageIndex < tutorialPages.Count - 1);
        if(currentPageIndex < tutorialMessages.Count)
        {
            tutorialText.text = tutorialMessages[currentPageIndex];
        }
    }
    //all texts in tutorialMessages List:
    public void firstPage()
    {
        tutorialMessages.Add("Hello dear traveler, my name is Naturactor! In this tutorial, I will guide you through the basics of the gameplay.");
    }
    public void secondPage()
    {
        tutorialMessages.Add("Your goal is to manage the elements of nature and keep the balance in the world. You will be able to build various structures and interact with the environment.");
    }
    public void thirdPage()
    {
        tutorialMessages.Add("To start, let's take a look at the map. You can move by using W,A,S,D on your keyboard. You can zoom in and out using the mouse wheel, and click and drag to move around.");
    }
    public void fourthPage()
    {
        tutorialMessages.Add("Now, let's build your first structure. Take a look at the building menu on the bottom of the screen. You can select different buildings to construct.");
    }
    public void fifthPage()
    {
        tutorialMessages.Add("Click on the map with your right mouse button. This will open the structure build option.");
    }
    public void sixthPage()
    {
        tutorialMessages.Add("Now place the structure on the map by using your left mouse button and clicking on the desired location. Remember: You can't place structures on water or other obstacles!");
    }
    public void seventhPage()
    {
        tutorialMessages.Add("Great! Now that you have built your first structure, let's take a look at the resources. You can see the resources on the top of the screen.");
    }
    public void eighthPage()
    {
        tutorialMessages.Add("These resources are essential for building and upgrading structures. Make sure to keep an eye on them and manage them wisely.");
    }
    public void ninthPage()
    {
        tutorialMessages.Add("Now, to the interesting part! You can interact with the environment and change the elements. For example, you can craft structures and affect fire, water, earth, and air and disbalance them. Keep an eye on that! Disasters may not be averted!.");
    }
    public void tenthPage()
    {
        tutorialMessages.Add("Oh, oh! I almost forgot! My dream is to find the once lost element of nature - the Void! Do you want to hear more about it?");
    }
    public void eleventhPage()
    {
        tutorialMessages.Add("Long ago, when heavens of the reactors were created, the Void was a part of the balance. But one day, it was lost in the depths of the universe.");
    }
    void twelfthPage()
    {
        tutorialMessages.Add("I believe that if we work together, we can find the Void and restore the full balance of nature and restore my once lost strength! I will help you with that! I will be your guide and assistant in this journey.");
    }
}