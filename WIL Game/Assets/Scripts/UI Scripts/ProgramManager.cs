using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{

    public static ProgramManager ProgramManagerInstance;

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
