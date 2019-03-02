using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    public GameObject battlebutton;
    public GameObject cancelButton;
    public GameObject connectingobject;
    public GameObject exitButton;
    public Text lobbyInfoText;

    private void Awake()
    {
        lobby = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();//connects to master photon server.
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player Connected");
        PhotonNetwork.AutomaticallySyncScene = true;
        battlebutton.SetActive(true);
        exitButton.SetActive(true);
        connectingobject.SetActive(false);     
    }

    public void OnBattleButtonClicked()
    {
        Debug.Log("Battle button was clicked");
        battlebutton.SetActive(false);
        exitButton.SetActive(false);
        cancelButton.SetActive(true);       
        PhotonNetwork.JoinRandomRoom();
        lobbyInfoText.gameObject.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join but failed");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Trying to create a new room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("room" + randomRoomName, roomOps);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("You are now in a room!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a new room");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");
        cancelButton.SetActive(false);
        battlebutton.SetActive(true);
        exitButton.SetActive(true);
        lobbyInfoText.gameObject.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
