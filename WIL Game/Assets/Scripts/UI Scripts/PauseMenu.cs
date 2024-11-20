using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject PausePanel;

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowOptions(bool ActiveState)
    {
        OptionsPanel.SetActive(ActiveState);
    }

    public void SetPauseState(bool PauseMenuActive)
    {
        PausePanel.SetActive(PauseMenuActive);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
