using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTesting : MonoBehaviour
{
    public int CycleNum;
    public int CycleCount;

    public float AttackWaitTime;
    public float ElapsedTime;

    public bool CycleAttacks;
    public bool CanAttack=true;

    public GameObject NormalCamera;
    public GameObject AimCamera;
    public GameObject Player;
    public GameObject Reader;
    public Vector3 RRoation;

    public float MouseSensitivity = 100f;
    public Transform CameraTransform;
    public Transform FacingCamera;

    public float RotationX = 0f;
    public float RotationY = 0f;

    public enum AllAttacks
    {
        None,
        SlashAttack,
        TailWhip,
        BiteAttack
    }
    public AllAttacks CurrentAttack;
    public AllAttacks NextAttack;

    public PlayerAttacks PlayerAttackScript;
    public CameraFunctionality CameraFunctionalityScript;
    public PlayerMovement PlayerMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAttackScript = GetComponent<PlayerAttacks>();
        PlayerMoveScript = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CycleAttacks = true;
            ElapsedTime = AttackWaitTime;
            CycleCount = CycleNum;
        }
        if (CycleAttacks)
        {
            //CycleAllAttacks();
        }

        if(Input.GetMouseButtonDown(1))
        {
            //CameraFunctionalityScript.CameraActive = false;
            //HandleCameraChange(AimCamera, true);
            //HandleCameraChange(NormalCamera, false);
            
        }
        ReadValues();
        return;
        if (AimCamera.activeSelf)
        {
            //FacingCamera.rotation = Quaternion.Euler(CameraTransform.eulerAngles.z, CameraTransform.eulerAngles.y, CameraTransform.eulerAngles.z);
            RotateCamera();
        }
        if(!AimCamera.activeSelf)
        {
            CameraTransform.rotation = Quaternion.LookRotation(PlayerMoveScript.BaltoOrientation.right);
            RotationX = PlayerMoveScript.BaltoOrientation.eulerAngles.x;
            RotationY = PlayerMoveScript.BaltoOrientation.eulerAngles.y;
        }
        if(Input.GetMouseButtonUp(1))
        {
            //CameraFunctionalityScript.CameraActive = true;
            
            //HandleCameraChange(NormalCamera, true);
            //HandleCameraChange(AimCamera, false);
            
        }
    }

    void ReadValues()
    {
        Reader.transform.rotation=Quaternion.Euler(RRoation.x, RRoation.y, RRoation.z);
        Vector3 localRotation = Reader.transform.localEulerAngles;

        localRotation.x = (localRotation.x > 180) ? localRotation.x - 360 : localRotation.x;
        localRotation.y = (localRotation.y > 180) ? localRotation.y - 360 : localRotation.y;
        localRotation.z = (localRotation.z > 180) ? localRotation.z - 360 : localRotation.z;
        Debug.Log(localRotation);
        //RRoation = new Vector3(Reader.transform.localRotation.eulerAngles.x, Reader.transform.localRotation.eulerAngles.y, Reader.transform.localRotation.eulerAngles.z);
        //RRoation = Quaternion.Euler(transform.localRotation).eulerAngles;
    }

    private void HandleCameraChange(GameObject ChangeCamera, bool State)
    {

        ChangeCamera.SetActive(State);
        ChangeCamera.GetComponent<Camera>().enabled = State;
        ChangeCamera.GetComponent<AudioListener>().enabled = State;
    }
    

    private void RotateCamera()
    {
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        //RotationY = CameraTransform.rotation.y;

        RotationX -= MouseY;

        //RotationX = Mathf.Clamp(RotationX, -90f, 90f);

        RotationY += MouseX;


        CameraTransform.rotation = Quaternion.Euler(RotationX, RotationY+90, 0f);
        FacingCamera.rotation = Quaternion.Euler(RotationX, RotationY + 90, 0f);
    }

}
