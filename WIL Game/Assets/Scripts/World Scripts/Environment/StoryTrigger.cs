using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{

    private WorldHandler WorldHandlerScript;


    [SerializeField] private string StoryText; 

    private void Start()
    {
        WorldHandlerScript = FindObjectOfType<WorldHandler>();
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            ///display the e to interact message to the player
            ///switch the player to interact keyboard binds
            Cursor.lockState = CursorLockMode.None;
            WorldHandlerScript.CurrentMode = WorldHandler.GameModes.Story;
            WorldHandlerScript.ModeChange.Invoke();
            ///show the mouse 
            ///make the escape key to exit the story telling mode
            ///add a pause button at the top
        }
    }

}
