using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    public float speed = 10;

    public GameObject blast;

    private Rigidbody rb;

    private MeshRenderer meshRend;

    private float dispAmount;

    private float movementX;
    private float movementY;

    private GameManager gameManager;

    public GameObject particlePrefab;
    private GameObject particle;

    public static bool IsPaused;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        rb = GetComponent<Rigidbody>();

        meshRend = GetComponent<MeshRenderer>();

        dispAmount = meshRend.material.GetFloat("_Amount");

        StartCoroutine(Dissolve());
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0, movementY);

        rb.AddForce(movement * speed);

        if (transform.position.y < 0 && !gameManager.IsWon())
        {
            gameManager.Lost();
        }

    }

    private void Update()
    {
        dispAmount = Mathf.Lerp(dispAmount, 0, Time.deltaTime);
        meshRend.material.SetFloat("_Amount", dispAmount);
    }


    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnCancel()
    {
        if (IsPaused)
        {
            gameManager.Resume();
        }
        else
        {
            gameManager.Pause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            CreateBlast(other.gameObject.transform.position);

            Destroy(other.gameObject);

            gameManager.AddScore();

            dispAmount += 1;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall") && !gameManager.IsWon())
        {
            other.gameObject.GetComponent<Renderer>().material.color = Color.red;
            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void CreateBlast(Vector3 position)
    {
        GameObject ablast = Instantiate(blast, position, Quaternion.identity);
        Destroy(ablast, 0.5f);
    }

    IEnumerator Dissolve()
    {
        particle = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        for (int i = 100; i > 0; i--)
        {
            meshRend.material.SetFloat("_Dissolve", i / 100f);
            yield return new WaitForSeconds(0.0075f);
        }
    }
}
