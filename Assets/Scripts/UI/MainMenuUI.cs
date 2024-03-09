using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject controlPanel;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowControls()
    {
        controlPanel.SetActive(true);
    }

    public void DisableControls()
    {
        controlPanel.SetActive(false);
    }

}
