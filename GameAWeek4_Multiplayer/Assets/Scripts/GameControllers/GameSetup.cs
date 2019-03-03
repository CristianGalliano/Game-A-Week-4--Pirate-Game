using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;

    public Transform[] spawnpoints;
    public List<int> playerIDList;
    public List<GameObject> livingPlayers;
    private bool canWinFlag = false;

    private void OnEnable()
    {
        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
    void Update()
    {
        if (!canWinFlag && livingPlayers.Count >= 2)
            canWinFlag = true;
        if (canWinFlag)
        {
            if (livingPlayers.Count == 1)
            {
                livingPlayers[0].GetComponent<PlayerMovement>().WinGame();
            }
        }
    }

    public void RemovePlayer(int playerID)
    {
        for (int i = 0; i < livingPlayers.Count; i++)
        {
            if (livingPlayers[i].GetComponent<PlayerMovement>().playerID == playerID)
            {
                livingPlayers.RemoveAt(i);
                playerIDList.Remove(playerID);
            }
        }
    }
}
