using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private PlayerInteraction PlayerInteractionScript;
    private int Counter = 0;
    [SerializeField] private string[] Lines = new string[6];

    private void Start()
    {
        Counter = 0;
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (!this.gameObject.activeSelf) { return; }

        if (Collision.CompareTag("Player"))
        {
            PlayerInteractionScript = FindObjectOfType<PlayerInteraction>();
            PlayerInteractionScript.StoryEnd();
            UpdateStoryText();
        }

    }

    public void UpdateStoryText()
    {
        if(Counter<Lines.Length)
        {
            PlayerInteractionScript.EndStoryText.text = Lines[Counter];
            PlayerInteractionScript.DarkenScreen();
            Counter++;
        }
        else if(Counter>=Lines.Length)
        {
            SceneManager.LoadScene("Main Menu"); 
        }
    }

}
