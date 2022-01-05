using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public GameObject menuPanel;
    public Text LoadingText;

    public GameObject createRoomScreen;
    public InputField RoomNameField;

    public GameObject RoomPanel;
    public Text roomNameText;

    public GameObject errorScreen;
    public Text ErrorText;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {/* 1.Close all menus
        2.When waiting for a connection have a LoadingScreen 
        3.ConnectUsingSettings
      */
        CloseMenus();

        loadingScreen.SetActive(true);
        LoadingText.text = "Connecting to Network. . .";

        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame

    void CloseMenus()
    {
        //whatever menus we have set them to be inactive intitially
        loadingScreen.SetActive(false);
        menuPanel.SetActive(false);
        createRoomScreen.SetActive(false);
        RoomPanel.SetActive(false);
        errorScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {/*
       4.Join Lobby
       5.waiting text wont kill
      */
        PhotonNetwork.JoinLobby();
        LoadingText.text = "Joining Lobby..";
    }

    public override void OnJoinedLobby()
    {/*
       6.In the lobby thats where we need to se the menu Buttons
            1.1st close all the menus
            2.set active the menuPanel only
      */

        CloseMenus();
        menuPanel.SetActive(true);
    }
    public void OpenCreateRoomScreen()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    public void CreateRoom()
    {//if the field is not empty
        if (!string.IsNullOrEmpty(RoomNameField.text))
        {
            //limit number of players in a room
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(RoomNameField.text,options);

            CloseMenus();
            LoadingText.text = "Creating Room. . .";
            loadingScreen.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        RoomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Failed to create room: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        menuPanel.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        LoadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);

    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuPanel.SetActive(true);
    }
}
