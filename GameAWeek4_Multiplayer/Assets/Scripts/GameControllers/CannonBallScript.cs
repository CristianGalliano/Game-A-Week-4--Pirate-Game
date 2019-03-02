using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CannonBallScript : MonoBehaviour
{
    private PhotonView PV;
    private Vector3 mousePos;
    private Vector3 originalPos;
    private Vector3 direction;
    private Rigidbody2D rb;
    private float highestSpeed = 0;
    public float speed;
    public float stopSpeed;
    private Camera cam;

    Vector2 hitPosition = Vector2.zero;
    public Tilemap tilemap;
    Collision2D collision1;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        tilemap = GameObject.Find("Land").GetComponent<Tilemap>();
        if (PV.IsMine)
        {
            cam = gameObject.transform.parent.transform.Find("Camera").GetComponent<Camera>();
            rb = gameObject.GetComponent<Rigidbody2D>();
            originalPos = transform.position;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            originalPos = new Vector3(originalPos.x, originalPos.y, 0);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            direction = Vector3.Normalize(mousePos - originalPos);
            Debug.Log(direction);
            gameObject.transform.parent = null;
            CannonBallMovement();
        }
    }

    private void CannonBallMovement()
    {
        rb.AddForce(direction * speed);
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (rb.velocity.magnitude > highestSpeed)
                highestSpeed = rb.velocity.magnitude;
            PV.RPC("cannonBallShrink", RpcTarget.All);
            if (rb.velocity.magnitude < stopSpeed && rb.velocity.magnitude != 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Land")
        {

            tilemap = collision.collider.GetComponent<Tilemap>();
            collision1 = collision;
            PV.RPC("removeTile", RpcTarget.All);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void removeTile()
    {
        foreach (ContactPoint2D hit in collision1.contacts)
        {
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

        }
        tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
    }

    [PunRPC]
    private void cannonBallShrink()
    {
        Vector3 newScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Sin((Mathf.PI / 2) * (rb.velocity.magnitude / highestSpeed)));
        if (float.IsNaN(newScale.x) || float.IsNaN(newScale.y) || float.IsNaN(newScale.z))
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            Debug.Log(newScale);
            transform.localScale = new Vector3(newScale.x, newScale.y, 1);
            //transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, rb.velocity.magnitude / highestSpeed);
        }
    }
}
