using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    //Button reference as a button press event on the UIMainMenu object
    public void HostLobby()
    {

        //Start host runs the game as a client and server at the same time
        networkManager.StartHost();

        //Takes you through to the page that has player's and ready status
        landingPagePanel.SetActive(false);
    }
}
