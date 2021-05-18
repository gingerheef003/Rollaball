using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGraphics : MonoBehaviour
{

    public float speed = 0.01f;
    private float uplim, downlim;
    public int mul = 1;

    void Start()
    {
        uplim = 0.6f;
        downlim = 0.4f;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime, Space.World);
        if(transform.position.y > uplim) { mul = -1; }
        if(transform.position.y < downlim) { mul = 1; }
        transform.position = transform.position + new Vector3(0, mul * speed, 0);
    }
}
