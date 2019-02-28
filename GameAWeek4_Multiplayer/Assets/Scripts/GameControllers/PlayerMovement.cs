using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float rotSpeed;
    private float rotSpeedPriv;
    public float dragCoefficient;
    public float angularDragCoefficient;
    private PhotonView PV;
    private Vector3 currentRotation;
    private Rigidbody2D rb;


    private Vector3 newPos;
    private Quaternion newRot;

    public Camera myCam;
    public AudioListener myAL;


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = gameObject.GetComponent<Rigidbody2D>();
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
            rb.AddRelativeForce(new Vector2(0, movementSpeed));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotSpeedPriv = rotSpeed * rb.velocity.magnitude;
            rb.AddTorque(rotSpeedPriv);
            /*currentRotation = gameObject.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation.z + rotSpeed * rb.velocity.magnitude);*/
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(new Vector2(0, -movementSpeed));
            /*newPos = new Vector3(transform.position.x, transform.position.y - movementSpeed, transform.position.z);
            transform.position = newPos;*/
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotSpeedPriv = rotSpeed * rb.velocity.magnitude;
            rb.AddTorque(-rotSpeedPriv);
            /*currentRotation = gameObject.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation.z - rotSpeed * rb.velocity.magnitude);*/
        }
        rb.drag = Mathf.Abs(rb.velocity.magnitude * dragCoefficient);
        rb.angularDrag = Mathf.Abs(rb.angularVelocity * angularDragCoefficient);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
