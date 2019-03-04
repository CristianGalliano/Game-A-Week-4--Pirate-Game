using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public int playerID;
    public string playerNickname = "";
    public float timeAlive = 0;
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
    public Canvas HUDCanvas, PauseMenuCanvas;
    public bool inPm = false;
    public AudioClip[] clips;
    public AudioSource thisAudiosource;
    private bool playSound = false;


    private Vector3 newPos;
    private Quaternion newRot;

    public Camera myCam;
    public AudioListener myAL;

    private GameObject cannonBall;
    public float cannonCooldown;
    private float cooldownTimeLeft;
    private float cooldownTimeRight;
    private bool canFireLeft = true;
    private bool canFireRight = true;

    private bool shot = false;
    private int ID;

    public float localForwardVelocity;

    public PolygonCollider2D limiter;

    public Image cannonballcooldownLeft;
    public Image cannonballcooldownRight;


    // Start is called before the first frame update
    void Start()
    {
        playerID = Random.Range(0, int.MaxValue);
        GameSetup.GS.playerIDList.Add(playerID);
        GameSetup.GS.livingPlayers.Add(gameObject);
        GameSetup.GS.namesList.Add("Player " + playerID.ToString().Substring(0,3));
        GameSetup.GS.timesList.Add(0);
        PV = GetComponent<PhotonView>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        health = initialHealth;
        cooldownTimeLeft = cannonCooldown; cooldownTimeRight = cannonCooldown;
        if (!PV.IsMine)
        {
            Destroy(myCam);
            Destroy(myAL);
            Destroy(HUDCanvas.gameObject);
            Destroy(PauseMenuCanvas.gameObject);
        }
        if (thisAudiosource == null)
        {
            PV.RPC("AddAS", RpcTarget.All);
        }

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            BasicMovement();
            timeAlive += Time.deltaTime;
        }
    }

    private void BasicMovement()
    {
        rb.drag = Mathf.Abs(rb.velocity.magnitude * dragCoefficient);
        rb.angularDrag = Mathf.Abs(rb.angularVelocity * angularDragCoefficient);
        localForwardVelocity = Vector3.Dot(rb.velocity, transform.up);
        if (inPm == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0);

                if (Vector2.Distance(mousePos, cannonballSpawns[0].transform.position) < Vector2.Distance(mousePos, cannonballSpawns[1].transform.position))
                {
                    if (canFireLeft)
                    {
                        cannonBall = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CannonBall"), cannonballSpawns[0].transform.position, Quaternion.identity);
                        canFireLeft = false;
                        cooldownTimeLeft = 0;
                        cannonballcooldownLeft.rectTransform.sizeDelta = new Vector2(0, cannonballcooldownLeft.rectTransform.sizeDelta.y);
                        cannonBall.transform.parent = gameObject.transform;
                        shot = true;
                        playSound = true;
                        PV.RPC("PlayCannonSound", RpcTarget.All);
                    }
                }
                else
                {
                    if (canFireRight)
                    {
                        cannonBall = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CannonBall"), cannonballSpawns[1].transform.position, Quaternion.identity);
                        canFireRight = false;
                        cooldownTimeRight = 0;
                        cannonballcooldownRight.rectTransform.sizeDelta = new Vector2(0, cannonballcooldownRight.rectTransform.sizeDelta.y);
                        cannonBall.transform.parent = gameObject.transform;
                        shot = true;
                        playSound = true;
                        PV.RPC("PlayCannonSound", RpcTarget.All);
                    }
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
        if (!canFireLeft)
        {
            cooldownTimeLeft += Time.deltaTime;
            cannonballcooldownLeft.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 1000, cooldownTimeLeft / cannonCooldown), cannonballcooldownLeft.rectTransform.sizeDelta.y);
            if (cooldownTimeLeft > cannonCooldown)
            {
                canFireLeft = true;
                cooldownTimeLeft = cannonCooldown;
            }
        }
        if (!canFireRight)
        {
            cooldownTimeRight += Time.deltaTime;
            cannonballcooldownRight.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 1000, cooldownTimeRight / cannonCooldown), cannonballcooldownRight.rectTransform.sizeDelta.y);
            if (cooldownTimeRight > cannonCooldown)
            {
                canFireRight = true;
                cooldownTimeRight = cannonCooldown;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inPause();
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
            GameSetup.GS.RemovePlayer(playerID);
            //ADD CODE FOR GAME LOSE HERE
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void RPC_AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }

    public void DisconnectPlayer()
    {
        GameSetup.GS.RemovePlayer(playerID);
        Destroy(PhotonRoom.room.gameObject);
        Destroy(GameSetup.GS.gameObject);
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

    public void inPause()
    {
        if (inPm == false)
        {
            HUDCanvas.gameObject.SetActive(false);
            PauseMenuCanvas.gameObject.SetActive(true);
            inPm = true;
        }
        else
        {
            HUDCanvas.gameObject.SetActive(true);
            PauseMenuCanvas.gameObject.SetActive(false);
            inPm = false;
        }
    }

    public void WinGame()
    {
        //add code for winning here
    }

    [PunRPC]
    private void PlayCannonSound()
    {
        thisAudiosource.Play();
    }

    [PunRPC]
    private void AddAS()
    {
        thisAudiosource = gameObject.AddComponent<AudioSource>();
        thisAudiosource.clip = clips[0];
        thisAudiosource.volume = 0.25f;
        thisAudiosource.rolloffMode = AudioRolloffMode.Linear;
        thisAudiosource.spatialBlend = 1;
        thisAudiosource.maxDistance = 100;
        thisAudiosource.minDistance = 1;
    }
}
