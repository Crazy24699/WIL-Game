
using TMPro;
using UnityEngine;
using UnityEngine.Audio;


public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private bool LastObjective;
    private bool Triggered;

    [SerializeField] private GameObject NextObjective;

    public AudioMixer MasterMixerRef;
    public TutorialHandler TutHandlerScript;
    private TextMeshProUGUI TutorialText;

    [SerializeField] private string UI_Text;

    private void Start()
    {
        TutHandlerScript = FindObjectOfType<TutorialHandler>();
        TutorialText = TutHandlerScript.Player_HUD_Text;
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if(Triggered || LastObjective) { return; }
        if (Collision.CompareTag("Player"))
        {
            NextObjective.SetActive(true);
            TutorialText.text = UI_Text;
            this.gameObject.SetActive(false);
            Triggered = true;
        }

    }

}
