using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public GameObject PauseMenuUI;
    public GameObject ResultMenuUI;
    public GameObject MainMenuUI;
    public TextMeshProUGUI sliderValue;
    public GameObject scoreText;
    public TextMeshProUGUI scoreTextUI;
    public Slider slider;

    public GameObject pickupPrefab;

    public int numberOfPickups;

    public CinemachineVirtualCamera vcam2;

    public GameObject walls;
    public Material wallMat;

    public Transform pickups;

    public InputActionAsset input;
    private InputAction cancel;

    public GameObject playerPrefab;
    private GameObject inGamePlayer;

    private int score;
    private Vector3[] pos;
    private Quaternion[] rot;
    // Start is called before the first frame update
    void Start()
    {

        int i = 0;
        pos = new Vector3[4];
        rot = new Quaternion[4];
        foreach (Transform wall in walls.transform)
        {
            pos[i] = wall.GetComponent<Transform>().position;
            rot[i] = wall.GetComponent<Transform>().rotation;
            wall.GetComponent<Renderer>().material = wallMat;
            i++;
        }

        numberOfPickups = (int)slider.value;

        PauseMenuUI.SetActive(false);
        ResultMenuUI.SetActive(false);
        scoreText.SetActive(false);

        MainMenuUI.SetActive(true);
        SetSliderValue(slider.value);

        cancel = input.FindActionMap("Player").FindAction("Cancel");
        cancel.Disable();
    }

    public void SetSliderValue(float value)
    {
        sliderValue.text = value.ToString();
    }

    void CreatePickups(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject apickup = Instantiate(pickupPrefab, new Vector3(Random.Range(-9f, 9f), -0.25f * Mathf.Sqrt(3) + Random.Range(-0.1f, 0.1f), Random.Range(-9f, 9f)), Quaternion.Euler(0, 0, 0), pickups) as GameObject;
            apickup.GetComponent<Transform>().Rotate(new Vector3(45, 45, 45), Space.World);
        }
    }

    void CreatePlayer()
    {
        Destroy(inGamePlayer);
        inGamePlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }

    public void AddScore()
    {
        score++;
        SetScoreText();
    }

    public bool IsWon()
    {
        return score == numberOfPickups;
    }

    public void Play()
    {
        score = 0;
        scoreText.SetActive(true);
        SetScoreText();

        CreatePlayer();

        foreach(Transform pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }
        numberOfPickups = (int)slider.value;
        CreatePickups((numberOfPickups));

        vcam2.Priority = 9;

        MainMenuUI.SetActive(false);
        ResultMenuUI.SetActive(false);


        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Renderer>().material = wallMat;
        }

        cancel.Enable();
    }

    public void Restart()
    {
        Resume();
        int i = 0;
        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Rigidbody>().useGravity = false;
            wall.GetComponent<Rigidbody>().isKinematic = true;
            wall.transform.position = pos[i];
            wall.transform.rotation = rot[i];
            i++;
        }

        foreach (Transform pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }

        Play();
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        PlayerController.IsPaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        PlayerController.IsPaused = true;
    }

    public void LoadMenuFromResult()
    {
        ResultMenuUI.SetActive(false);
        MainMenuUI.SetActive(true);
        scoreText.SetActive(false);
        Destroy(inGamePlayer);

        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Renderer>().material = wallMat;
        }

        cancel.Disable();
    }

    public void LoadMenuFromPause()
    {
        Resume();
        Destroy(inGamePlayer);
        MainMenuUI.SetActive(true);
        scoreText.SetActive(false);

        int i = 0;
        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Renderer>().material = wallMat;
            wall.GetComponent<Rigidbody>().useGravity = false;
            wall.GetComponent<Rigidbody>().isKinematic = true;
            wall.transform.position = pos[i];
            wall.transform.rotation = rot[i];
            i++;
        }

        foreach (Transform pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }

        cancel.Disable();
    }

    private void SetScoreText()
    {
        scoreTextUI.text = "Score: " + score.ToString();

        if (score >= numberOfPickups)
        {
            Won();
        }
    }

    public void Lost()
    {
        ResultMenuUI.SetActive(true);
        ResultMenuUI.GetComponentInChildren<TextMeshProUGUI>().text = "You Lost";

        vcam2.Priority = 11;

        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Renderer>().material.color = Color.red;
            wall.GetComponent<Rigidbody>().useGravity = false;
            wall.GetComponent<Rigidbody>().isKinematic = true;
        }

        StartCoroutine(WallReset());
    }

    public void Won()
    {
        ResultMenuUI.SetActive(true);
        ResultMenuUI.GetComponentInChildren<TextMeshProUGUI>().text = "You Won";

        vcam2.Priority = 11;

        foreach (Transform wall in walls.transform)
        {
            wall.GetComponent<Renderer>().material.color = Color.green;
            wall.GetComponent<Rigidbody>().useGravity = false;
            wall.GetComponent<Rigidbody>().isKinematic = true;
        }

        StartCoroutine(WallReset());
    }

    IEnumerator WallReset()
    {
        for (int j = 0; j < 1000; j++)
        {
            int i = 0;
            foreach (Transform wall in walls.transform)
            {
                wall.GetComponent<Transform>().position = Vector3.Lerp(wall.GetComponent<Transform>().position, pos[i], j / 1000f);
                wall.GetComponent<Transform>().rotation = Quaternion.Lerp(wall.GetComponent<Transform>().rotation, rot[i], j / 1000f);
                i++;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }
}
