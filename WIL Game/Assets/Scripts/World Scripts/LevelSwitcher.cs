using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitcher : MonoBehaviour
{

    [SerializeField] private string NextScene;

    private void OnTriggerEnter(Collider Collision)
    {
        if (!this.gameObject.activeSelf) { return; }

        if (Collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(NextScene);
        }

    }

}
