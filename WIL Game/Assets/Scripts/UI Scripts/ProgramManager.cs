using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProgramManager : MonoBehaviour
{

    public static ProgramManager ProgramManagerInstance;
    public bool GamePaused = false;

    public void Start()
    {
        if(ProgramManagerInstance == null)
        {
            ProgramManagerInstance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Debug.Log("got one");
        
    }


    private void Update()
    {
        switch (Time.timeScale)
        {
            case <= 0:
                GamePaused = true;
                break;

            case >0:
                GamePaused = false;
                break;
        }
    }

}

public static class ExtensionClass
{
    public static Vector3 RoundVector(this Vector3 Vector3Ref, int DecimalRound)
    {
        Vector3 VectorOut = Vector3.zero;

        float DecimalMultiplier = 1;
        for (int i = 0; i < DecimalRound; i++)
        {
            DecimalMultiplier *= 10f;
        }

        VectorOut.x = Mathf.Round(Vector3Ref.x * DecimalMultiplier) / DecimalMultiplier;
        VectorOut.y = Mathf.Round(Vector3Ref.y * DecimalMultiplier) / DecimalMultiplier;
        VectorOut.z = Mathf.Round(Vector3Ref.z * DecimalMultiplier) / DecimalMultiplier;

        return VectorOut;
    }

    public static float RoundFloat(this float FloatIn, int DecimalRound)
    {
        float FloatOut = 0.0f;

        float DecimalMultiplier = 1;
        for (int i = 0; i < DecimalRound; i++)
        {
            DecimalMultiplier *= 10f;
        }
        FloatOut = Mathf.Round(FloatIn * DecimalMultiplier) / DecimalMultiplier;

        return FloatOut;
    }

    public static Vector3 NegativeRotation(this Vector3 Vector3Ref, GameObject ObjectTransform)
    {
        Vector3 UnityEditorRotation = ObjectTransform.transform.localEulerAngles;

        UnityEditorRotation.x = (UnityEditorRotation.x > 180) ? UnityEditorRotation.x - 360 : UnityEditorRotation.x;
        UnityEditorRotation.y = (UnityEditorRotation.y > 180) ? UnityEditorRotation.y - 360 : UnityEditorRotation.y;
        UnityEditorRotation.z = (UnityEditorRotation.z > 180) ? UnityEditorRotation.z - 360 : UnityEditorRotation.z;
        return UnityEditorRotation;
    }
}

