using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleProjectile : ProjectileBase
{

    [SerializeField]private float CurrentLifeTime = 0.0f;
    private bool StartedFadeEffect;

    [SerializeField]private Vector2 InitialBubbleSize;
    [SerializeField] private Vector2 FinalBubbleSize;

    [SerializeField] private Material BubbleMaterialRef;
    private Material BubbleMaterialClone;
    private Color MaterialColour;
    protected override void CustomBehaviour()
    {
        Damage = 1;
        LifeTime = 8;
        CurrentLifeTime = LifeTime;

        BubbleMaterialClone = new Material(BubbleMaterialRef);
        BubbleMaterialClone.name = "BubbleMaterial clone";
        this.GetComponent<MeshRenderer>().material = BubbleMaterialClone;

        InitialBubbleSize = transform.localScale;
        FinalBubbleSize = Vector3.one * 2;

        CustomLifeTimer = true;
        MaterialColour = BubbleMaterialRef.color;


    }

    private void Start()
    {
        //CustomBehaviour();
    }

    //there are 2 ways to do this, one via animation and one via code. Just checking when the alpha of the
    //colour is completely gone, that is easier but i like code so im doing it this way
    private void Update()
    {
        if (CurrentLifeTime <= 0)
        {
            CurrentLifeTime = 0;
            return;
        }

        CurrentLifeTime -= Time.deltaTime;

        if (transform.lossyScale.x < FinalBubbleSize.x)
        {
            float SizeIncriment = 1 - CurrentLifeTime / LifeTime;
            float XScaleChange = Mathf.Lerp(InitialBubbleSize.x, FinalBubbleSize.x, SizeIncriment);

            Vector3 BubbleScaleChange = Vector3.one * XScaleChange;
            this.transform.localScale = BubbleScaleChange;
        }

        if (CurrentLifeTime <= LifeTime / 2)
        {
            //How to decriment a lerp value 
            float FadeValue = 1 - (CurrentLifeTime / (LifeTime / 2));
            

            //BubbleMaterialRef.color = MaterialColour;

            MaterialColour.a = Mathf.Lerp(1, 0, FadeValue);
            BubbleMaterialClone.color = MaterialColour;
            this.GetComponent<MeshRenderer>().material = BubbleMaterialClone;
            //Debug.Log("worl turns black     " + BubbleMaterialClone.color.a + "     " + this.GetComponent<MeshRenderer>().material.color.a);


        }

        if (CurrentLifeTime <= 0)
        {
            Destroy(gameObject);
        }


    }


    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            Collision.GetComponent<PlayerInteraction>().HandleHealth(-1);
        }
        Destroy(gameObject);
    }

}
