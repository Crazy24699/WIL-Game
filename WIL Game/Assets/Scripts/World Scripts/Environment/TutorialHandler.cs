using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public TextMeshProUGUI Player_HUD_Text;

    [SerializeField] private BaseEnemy FirstTarget;
    private bool ShardExplained = false;

    private void Start()
    {
        Player_HUD_Text.text = "Press W,A,S,D to move, you are able to press space to dash in a direction, proceed \nto the waypoint";
    }


    private void Update()
    {
        if (FirstTarget == null && !ShardExplained)
        {
            Player_HUD_Text.text = "Enemies will drop shards, you need shards to heal and progress through the game, be careful how you use them. Progress to the next waypoint";

            ShardExplained = true;
            //Debug.LogError("turn itaround");
        }
    }

}
