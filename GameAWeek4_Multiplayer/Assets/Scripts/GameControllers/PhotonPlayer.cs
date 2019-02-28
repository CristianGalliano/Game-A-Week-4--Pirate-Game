using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        int spawnpicker = Random.Range(0, GameSetup.GS.spawnpoints.Length);
        if (PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.GS.spawnpoints[spawnpicker].position, GameSetup.GS.spawnpoints[spawnpicker].rotation, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
