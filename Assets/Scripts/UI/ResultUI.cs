using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI hitCountText;

    void Awake()
    {
       
        if (PlayerPrefs.HasKey("HitCount"))
        {
            hitCountText.text = PlayerPrefs.GetInt("HitCount").ToString();
        }
        else
        {
            hitCountText.text = (-99).ToString();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void Exit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
