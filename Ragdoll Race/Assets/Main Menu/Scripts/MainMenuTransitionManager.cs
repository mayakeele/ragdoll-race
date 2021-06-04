using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTransitionManager : MonoBehaviour
{
    public GameObject firstScreen;
    public List<GameObject> menuScreens;


    private void Start()
    {
        ShowScreen(firstScreen);
    }


    public void ShowScreen(GameObject screenToShow){
        // Hides all UI screens, then activates selected screen

        foreach(GameObject screen in menuScreens){
            screen.SetActive(false);
        }

        screenToShow.SetActive(true);
    }


}
