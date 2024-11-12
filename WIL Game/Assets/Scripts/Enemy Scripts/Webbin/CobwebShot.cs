using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobwebShot : ProjectileBase
{
    [SerializeField] private GameObject Webbing;

    private bool IsProjectile = true;
    private bool StartupRan = false;

    private Vector3 StartingSize;
    private Vector3 EndSize;

    public void Startup()
    {
        StartingSize = transform.lossyScale;
        EndSize = StartingSize * 1.5f;

        StartupRan = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan) { return; }

    }


    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if (IsProjectile)
            {
                Collision.transform.root.GetComponent<PlayerInteraction>().HandleHealth(-Damage);
                Destroy(this.gameObject);
                return;
            }

        }

        if (Collision.CompareTag("Ground") && IsProjectile)
        {
            IsProjectile = false;
        }
    }

}
