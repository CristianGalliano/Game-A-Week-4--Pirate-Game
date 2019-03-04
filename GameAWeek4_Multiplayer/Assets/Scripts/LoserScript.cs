using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoserScript : MonoBehaviour
{
    public GameObject gameovercanvas;
    public Camera endgamecam;

    private bool showUI = false;
    private bool isCanvasActive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSetup.GS.canWinFlag)
        {
            if (Camera.current != null)
            {
                Debug.Log(Camera.current.Equals(endgamecam));
                if (Camera.current.Equals(endgamecam))
                {
                    showUI = true;
                }
            }
            if (showUI)
            {
                ShowEndUI();
            }
        }
    }

    private void ShowEndUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isCanvasActive == false)
            {
                gameovercanvas.SetActive(true);
                isCanvasActive = true;
            }
            else
            {
                gameovercanvas.SetActive(false);
                isCanvasActive = false;
            }
        }
    }

    public void DisconnectPlayer()
    {
        Destroy(PhotonRoom.room.gameObject);
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
