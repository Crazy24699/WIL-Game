using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WorldHandler : MonoBehaviour
{
    public UnityEvent ModeChange;
    public UnityEvent GamePause;
    public UnityEvent RunLoadingScreen;
    [SerializeField] private GameObject LoadingScreenPanel;
    [Space(1.2f), Header(" ")]
    [SerializeField] private GameObject PlayerStartPosition;
    public PlayerInteraction PlayerInteractionScript;

    public Dictionary<int, List<SpireObject>> AllSpires = new Dictionary<int, List<SpireObject>>();

    public List<GameObject> AllEnemies = new List<GameObject>();
    public HashSet<GameObject> EnemiesAttacking = new HashSet<GameObject>();
    public List<GameObject> VisableEnemies = new List<GameObject>();

    public GameObject Boss1;
    public GameObject FinalBoss;

    [Space(2)]
    public GameObject PlayerObject;

    public float CurrentTimescale;

    [SerializeField] private List<GameObject> Entities = new List<GameObject>();

    public enum GameModes
    {
        Story,
        Menu,
        Gameplay
    };

    public GameModes CurrentMode;

    private bool RunLoadscreen = false;

    void Start()
    {
        CurrentMode = GameModes.Gameplay;
        if (AllSpires.Count == 0)
        {

        }
        if (RunLoadscreen)
        {
            RunLoadingScreen.AddListener(() => StartCoroutine(HandleLoadingScreen()));

            RunLoadingScreen.Invoke();
        }

        //HandleLoadValues();
        
        //Move all spires into a different script
        //StartCoroutine(TempSetActiveEnemy());
        //Debug.Log(AllSpires.Count);
        //Debug.Log(AllSpires.ElementAt(0).Value.Count);

        ModeChange.AddListener(() => ChangeInputMode(CurrentMode));

    }


    public void ChangeInputMode(GameModes ChosenMode)
    {
        switch (ChosenMode)
        {
            case GameModes.Story:
                PlayerInteractionScript.HandleStoryState(true);

                break;

            case GameModes.Menu:
                PlayerInteractionScript.HandleStoryState(false);
                break;

            case GameModes.Gameplay:
                PlayerInteractionScript.HandleStoryState(false);
                break;
            
        }
    }

    private void HandleLoadValues()
    {
        PlayerObject.transform.position = PlayerStartPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTimescale = Time.timeScale;
        VisableEnemies=EnemiesAttacking.ToList();
    }

    private IEnumerator TempSetActiveEnemy()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Rise");
        for (int i = 0; i < AllEnemies.Count; i++)
        {
            AllEnemies[i].gameObject.SetActive(false);
            if (i == 0)
            {
                AllEnemies[0].gameObject.SetActive(true);
            }
        }
    }

    public void SetNextActive(GameObject RemovedEnemy)
    {
        AllEnemies.Remove(RemovedEnemy.GetComponent<GameObject>());
        if(AllEnemies.Count > 0)
        {
            int Rnd = Random.Range(0, AllEnemies.Count);
            AllEnemies[Rnd].gameObject.SetActive(true);
        }

    }

    public void SetActiveArea(string ActiveArea)
    {
        if (ActiveArea == "Boss 1 Area")
        {
            Boss1.SetActive(true);
        }

        if(ActiveArea =="Final Boss Area")
        {
            FinalBoss.SetActive(true);
        }
    }

    public async void ReloadPathfindingGrid()
    {
        await PathGridReloadTimer();
    }

    public async Task PathGridReloadTimer ()
    {
        await Task.Delay(100);
    }

    private IEnumerator HandleLoadingScreen()
    {
        
        yield return new WaitForSeconds(5.75f);
        LoadingScreenPanel.SetActive(false);
    }

}
