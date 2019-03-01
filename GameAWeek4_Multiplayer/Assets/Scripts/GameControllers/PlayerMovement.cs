using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private GameObject cannonBall;

    private bool shot = false;
    private int ID; 

    public float localForwardVelocity;


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
    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            BasicMovement();
        }
    }

    private void BasicMovement()
    {
        rb.drag = Mathf.Abs(rb.velocity.magnitude * dragCoefficient);
        rb.angularDrag = Mathf.Abs(rb.angularVelocity * angularDragCoefficient);
        localForwardVelocity = Vector3.Dot(rb.velocity, transform.up);
        if (Input.GetMouseButtonDown(0))
        {
            cannonBall = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CannonBall"), transform.position, Quaternion.identity);
            cannonBall.transform.parent = gameObject.transform;
            shot = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(new Vector2(0, movementSpeed));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotSpeedPriv = rotSpeed * localForwardVelocity;
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
            rotSpeedPriv = rotSpeed * localForwardVelocity;
            rb.AddTorque(-rotSpeedPriv);
            /*currentRotation = gameObject.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation.z - rotSpeed * rb.velocity.magnitude);*/
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Land")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
