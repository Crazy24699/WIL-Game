using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject OptionsMenuPanel;
    public GameObject MainMenuPanel;
    public GameObject PlayOptionPanel;
    public GameObject CreditsPanel;

    public GameObject PreviousPanel;
    public GameObject CurrentPanel;


    private void Start()
    {
        CurrentPanel = MainMenuPanel;
        OptionsMenuPanel.SetActive(false);
        PlayOptionPanel.SetActive(false);

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1 Greybox"); 
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void ShowOptions()
    {
        OptionsMenuPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        CurrentPanel = OptionsMenuPanel;
    }

    public void ShowPlayOptions()
    {
        PlayOptionPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        CurrentPanel = PlayOptionPanel;
    }

    public void BackButton(GameObject LastPanelRef)
    {
        LastPanelRef.SetActive(true);
        CurrentPanel.SetActive(false);
        CurrentPanel = LastPanelRef;
    }

    public void ShowCredits()
    {
        CreditsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        CurrentPanel = CreditsPanel;
    }

}
