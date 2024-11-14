using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickup : MonoBehaviour
{
    private GameObject GemBodyRef;
    private bool Uncounted = false;
    private void Start()
    {
        GemBodyRef = transform.GetComponentInChildren<MeshRenderer>().gameObject;
        this.gameObject.name = GemBodyRef.name + "" + Random.Range(-965, 956);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player") && Collision.gameObject.GetComponent<PlayerInteraction>() != null && !Uncounted)
        {
            //Play anim
            //hide the shard body
            //Destroy shard after a moment

            Collision.GetComponent<PlayerInteraction>().IncrimentGemCount();
            Uncounted = true;
            StartCoroutine(ShardInteraction());
        }
    }

    private IEnumerator ShardInteraction()
    {

        GemBodyRef.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.25f);
        Destroy(this.gameObject);
    }
}
