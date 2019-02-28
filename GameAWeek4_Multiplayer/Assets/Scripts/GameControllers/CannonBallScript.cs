using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    private PhotonView PV;
    private Vector3 mousePos;
    private Vector3 originalPos;
    private Vector3 direction;
    private Rigidbody2D rb;
    public float speed;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            cam = gameObject.transform.parent.transform.Find("Camera").GetComponent<Camera>();
            rb = gameObject.GetComponent<Rigidbody2D>();
            originalPos = transform.position;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            direction = Vector3.Normalize(mousePos - originalPos);
            gameObject.transform.parent = null;
            CannonBallMovement();
        }
    }

    private void CannonBallMovement()
    {
        rb.AddForce(direction * speed);
    }
}
