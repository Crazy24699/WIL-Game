using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField]public float Radius;
    [Range(0, 360)]
    [SerializeField] public float Angle;
    private float LostTimer = 3.5f;
    [SerializeField]private float LostTimePast;

    //public GameObject PlayerRef;

    public LayerMask TargetMask;
    public LayerMask ObstructionMask;

    private bool StartupRan = false;
    public bool CanSeePlayer;
    [SerializeField]private bool LostPlayer = false;
    [SerializeField]private BaseEnemy SightLinkScript;

    public void Startup(BaseEnemy LinkScript)
    {
        StartCoroutine(FOVRoutine());
        SightLinkScript = LinkScript;
        Debug.Log("this setup ran");
    }
    

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void Update()
    {
        if (!StartupRan) { return; }
        SightLinkScript.SeenPlayer = CanSeePlayer;
        if (Input.GetKeyDown(KeyCode.V))
        {
            LostPlayer = false;
            CanSeePlayer = false;
            StartCoroutine(HandleLostDelay());
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] RangeChecks = Physics.OverlapSphere(transform.position, Radius, TargetMask);
        Debug.Log("sing and grieve");
        if (RangeChecks.Length != 0)
        {
            Transform Target = RangeChecks[0].transform;
            Vector3 DirectionToTarget = (Target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, DirectionToTarget) < Angle / 2)
            {
                float DistanceToTarget = Vector3.Distance(transform.position, Target.position);

                if (!Physics.Raycast(transform.position, DirectionToTarget, DistanceToTarget, ObstructionMask))
                {
                    CanSeePlayer = true;
                    SightLinkScript.SeenPlayer = true;
                }
                else
                {
                    CanSeePlayer = false;
                    StartCoroutine(HandleLostDelay());
                }
            }
            else
            {
                CanSeePlayer = false;
                StartCoroutine(HandleLostDelay());
            }
        }
        else if (CanSeePlayer)
        {
            CanSeePlayer = false;
            StartCoroutine(HandleLostDelay());
        }
    }

    private IEnumerator HandleLostDelay()
    {
        LostTimePast = 0;
        yield return new WaitForSeconds(0.15f);
        if(CanSeePlayer )
        {
            yield break;
        }
        while (!LostPlayer && !CanSeePlayer)
        {
            //yield return new WaitForSeconds(0.1f);
            LostTimePast += Time.deltaTime;
            if (LostTimePast >= LostTimer)
            {
                LostPlayer = true;
                SightLinkScript.SeenPlayer = false;
            }
            yield return null;
        }
    }

}
