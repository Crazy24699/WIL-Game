using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolutionShard : MonoBehaviour
{
    private GameObject ShardBodyRef;
    private bool Uncounted = false;
    private void Start()
    {
        ShardBodyRef = transform.GetComponentInChildren<MeshRenderer>().gameObject;
        this.gameObject.name = ShardBodyRef.name + "" + Random.Range(-965, 956);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player") && Collision.gameObject.GetComponent<PlayerInteraction>() != null && !Uncounted)
        {
            //Play anim
            //hide the shard body
            //Destroy shard after a moment


            if(!Uncounted)
            {
                Uncounted = true;
                Collision.GetComponent<PlayerInteraction>().HandleShardUpdate();
                Destroy(this.gameObject);
                return;
            }

            StartCoroutine(ShardInteraction()); 
        }
    }

    private IEnumerator ShardInteraction()
    {
        if(!Uncounted) { yield break; }
        ShardBodyRef.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.25f);

    }

}
