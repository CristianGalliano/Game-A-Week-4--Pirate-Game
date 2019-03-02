using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float initialHealth;
    public float health;
    public float landDamageMultiplier;
    public float landBounceMultiplier;
    public float cannonballDamageMultiplier;
    public float cannonballBounceMultiplier;
    public float boatDamageMultiplier;
    public float boatBounceMultiplier;
    public float movementSpeed;
    public float rotSpeed;
    private float rotSpeedPriv;
    public float dragCoefficient;
    public float angularDragCoefficient;
    private PhotonView PV;
    private Vector3 currentRotation;
    private Rigidbody2D rb;
    public GameObject[] cannonballSpawns;
    public Canvas ourCanvas;


    private Vector3 newPos;
    private Quaternion newRot;

    public Camera myCam;
    public AudioListener myAL;

    private GameObject cannonBall;
    private bool canFire = true;
    public float cannonCooldown;

    private bool shot = false;
    private int ID;

    public float localForwardVelocity;

    public PolygonCollider2D limiter;

    public Image cannonballcooldown;


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        ourCanvas = gameObject.GetComponentInChildren<Canvas>();
        health = initialHealth;
        if (!PV.IsMine)
        {    
            Destroy(myCam);
            Destroy(myAL);
            Destroy(ourCanvas.gameObject);
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
            if (canFire)
            {
                Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0);

                if (Vector2.Distance(mousePos, cannonballSpawns[0].transform.position) < Vector2.Distance(mousePos, cannonballSpawns[1].transform.position))
                {
                    cannonBall = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CannonBall"), cannonballSpawns[0].transform.position, Quaternion.identity);
                }
                else
                {
                    cannonBall = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CannonBall"), cannonballSpawns[1].transform.position, Quaternion.identity);
                }
                cannonBall.transform.parent = gameObject.transform;
                shot = true;
                canFire = false;
                cannonballcooldown.rectTransform.sizeDelta = new Vector2(0, cannonballcooldown.rectTransform.sizeDelta.y);
                if (PV.IsMine)
                    StartCoroutine(CannonCooldown());
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(new Vector2(0, movementSpeed));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotSpeedPriv = rotSpeed * localForwardVelocity;
            rb.AddTorque(rotSpeedPriv);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(new Vector2(0, -movementSpeed));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotSpeedPriv = rotSpeed * localForwardVelocity;
            rb.AddTorque(-rotSpeedPriv);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Land")
        {
            if (PV.IsMine)
            {
                PV.RPC("DealDamage", RpcTarget.All,(rb.velocity.magnitude * landDamageMultiplier));
                PV.RPC("RPC_AddForce", RpcTarget.All,(-rb.velocity * landBounceMultiplier));
            }
        }
        if (collision.collider.tag == "CannonBall")
        {
            Rigidbody2D cannonBallRB = collision.collider.GetComponent<Rigidbody2D>();
            PV.RPC("DealDamage",RpcTarget.All,(cannonBallRB.velocity.magnitude * cannonballDamageMultiplier));
            PV.RPC("RPC_AddForce", RpcTarget.All,(cannonBallRB.velocity * cannonballBounceMultiplier));
            Debug.Log("test");
            PhotonNetwork.Destroy(cannonBallRB.gameObject);
        }
        if (collision.collider.tag == "Boat")
        {
            if (PV.IsMine)
            {
                Rigidbody2D enemyRB = collision.collider.GetComponent<Rigidbody2D>();
                Vector3 velocityDifference = enemyRB.velocity - rb.velocity;
                PV.RPC("DealDamage", RpcTarget.All, (Mathf.Abs(velocityDifference.magnitude) * boatDamageMultiplier));
                PV.RPC("RPC_AddForce", RpcTarget.All, (enemyRB.velocity * boatBounceMultiplier));
            }
        }
    }

    [PunRPC]
    private void DealDamage(float damage)
    {
        Debug.Log(health + " - " + damage + " = " + (health - damage));
        health -= damage;
        if (health <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void RPC_AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }

    IEnumerator CannonCooldown()
    {
        yield return new WaitForSeconds(cannonCooldown);
        canFire = true;
    }
}
