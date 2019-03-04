using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;

    public Transform[] spawnpoints;
    public List<int> playerIDList;
    public List<GameObject> livingPlayers;
    public Stack<string> playerNames = new Stack<string>();
    public Stack<float> playerTimes = new Stack<float>();
    public string winner;
    [HideInInspector]
    public bool canWinFlag = false;

    private void OnEnable()
    {
        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        winner = "WINNER NOT DEFINED";
    }

    void LateUpdate()
    {
        if (!canWinFlag && livingPlayers.Count >= 2)
            canWinFlag = true;
        if (canWinFlag)
        {
            if (livingPlayers.Count == 1)
            {
                winner = livingPlayers[0].GetComponent<PlayerMovement>().playerNickname;
                SceneManager.LoadScene(2);
            }
        }
    }

    public void RemovePlayer(int playerID)
    {
        Debug.Log("removing a player");
        for (int i = 0; i < livingPlayers.Count; i++)
        {
            if (livingPlayers[i].GetComponent<PlayerMovement>().playerID == playerID)
            {
                playerNames.Push(livingPlayers[i].GetComponent<PlayerMovement>().playerNickname);
                playerTimes.Push(livingPlayers[i].GetComponent<PlayerMovement>().timeAlive);
                Debug.Log(name + ", " + playerTimes.Peek());
                livingPlayers.RemoveAt(i);
                playerIDList.Remove(playerID);
            }
        }
    }
}
