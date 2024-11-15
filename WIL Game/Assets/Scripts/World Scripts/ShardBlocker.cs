using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShardBlocker : MonoBehaviour
{
    [SerializeField] private int MaxblockLevel;
    private int CurrentBlockLevel;

    [SerializeField] private TextMeshProUGUI RemainingCounter;

    [SerializeField]private PlayerInteraction PlayerInteractScript;
    [SerializeField]private bool PlayerInBounds = false;

    // Start is called before the first frame update
    void Start()
    {
        if(MaxblockLevel <= 0)
        {
            Debug.LogError("Block level not set");
            MaxblockLevel = 3;
        }
        CurrentBlockLevel = MaxblockLevel;
        RemainingCounter = transform.GetComponentInChildren<TextMeshProUGUI>();
        RemainingCounter.text = "Needed Gems: " + CurrentBlockLevel.ToString();
    }



    private void OnTriggerEnter(Collider Collider)
    {
        if(Collider.CompareTag("Player"))
        {
            Debug.Log("oae");

            PlayerInteractScript = Collider.transform.root.root.GetComponent<PlayerInteraction>();

        }
    }

    private void Update()
    {
        if (CurrentBlockLevel <= 0)
        {
            //Play animations 
            //play sound
            //play vfx
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider Collider)
    {
        if (Collider.CompareTag("Player") && PlayerInteractScript != null)
        {
            PlayerInteractScript.CurrentShardBlocker = this;
            PlayerInBounds = true;
            PlayerInteractScript.InBlockerRange = true;
        }
    }

    private void OnTriggerExit(Collider Collider)
    {
        if (Collider.CompareTag("Player") && PlayerInteractScript != null)
        {
            PlayerInBounds = false;
            PlayerInteractScript.InBlockerRange = false;
        }
    }

    public void RemoveBlockLevel()
    {
        if (!PlayerInBounds)
        {
            return;
        }
        CurrentBlockLevel--;
        RemainingCounter.text="Needed Gems: "+CurrentBlockLevel.ToString();
        if (CurrentBlockLevel <= 0)
        {
            //Play animations 
            //play sound
            //play vfx
            StartCoroutine(DestroyBlocker());
        }
    }
    private IEnumerator DestroyBlocker()
    {
        yield return new WaitForSeconds(1.35f);
        Destroy(this.gameObject);
    }
}
