using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    private float rot;
    private PhotonView PV;

    private Vector3 newPos;

    public Camera myCam;
    public AudioListener myAL;


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            Destroy(myCam);
            Destroy(myAL);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            BasicMovement();
        }
    }

    private void BasicMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            newPos = new Vector3(transform.position.x, transform.position.y + movementSpeed, transform.position.z);
            transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPos = new Vector3(transform.position.x - movementSpeed, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPos = new Vector3(transform.position.x, transform.position.y - movementSpeed, transform.position.z);
            transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPos = new Vector3(transform.position.x + movementSpeed, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
