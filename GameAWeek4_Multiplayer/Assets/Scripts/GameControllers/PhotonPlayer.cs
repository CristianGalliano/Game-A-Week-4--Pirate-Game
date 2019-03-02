using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    private bool validSpawnCheck = false;
    public float spawnBoundLeft;
    public float spawnBoundRight;
    public float spawnBoundUp;
    public float spawnBoundDown;
    private float spawnPointX;
    private float spawnPointY;
    public float spawnZoneSize;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        while (!validSpawnCheck)
        {
            validSpawnCheck = false;
            spawnPointX = (spawnBoundRight - spawnBoundLeft) * Random.value + spawnBoundLeft;
            spawnPointY = (spawnBoundUp - spawnBoundDown) * Random.value + spawnBoundDown;
            if (!((spawnPointX > (spawnBoundLeft + spawnZoneSize) && spawnPointX < (spawnBoundRight - spawnZoneSize))
                && (spawnPointY > (spawnBoundDown + spawnZoneSize) && spawnPointY < (spawnBoundUp - spawnZoneSize))))
            {
                validSpawnCheck = true;
            }
            if (count > 100)
                validSpawnCheck = true;
            count++;
        }
        if (PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), new Vector3(spawnPointX, spawnPointY), Quaternion.identity, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
