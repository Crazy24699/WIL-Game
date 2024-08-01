using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProgramManager : MonoBehaviour
{

    public static ProgramManager ProgramManagerInstance;
    public InputActionReference PlayerActionMap;

    public void Start()
    {
        if(ProgramManagerInstance == null)
        {
            ProgramManagerInstance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(ProgramManagerInstance);
    }


}
