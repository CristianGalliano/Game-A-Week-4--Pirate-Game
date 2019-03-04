using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerScript : MonoBehaviour
{
    public Text winnerText;
    string buffer = "";
    GameSetup dataStore;
    // Start is called before the first frame update
    void Start()
    {
        buffer = "Game over!\nYour winner is...\n";
        buffer += GameSetup.GS.winner + "\n\n";
        buffer += "An ode to the lost...\n";
        while (GameSetup.GS.playerNames.Count > 0)
        {
            buffer += GameSetup.GS.playerNames.Pop() + ", survived for " + Mathf.Floor(GameSetup.GS.playerTimes.Pop()) + " seconds.\n";
        }

        winnerText.text = buffer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisconnectPlayer()
    {
        Destroy(PhotonRoom.room.gameObject);
        Destroy(GameSetup.GS.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.menuScene);
    }
}
