using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallFollow : MonoBehaviour
{
    private Vector3 position;
    private GameObject boat;
    // Start is called before the first frame update
    void Start()
    {
        boat = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }

    private void Follow()
    {
        position = boat.transform.position;
        transform.position = position;
    }
}
