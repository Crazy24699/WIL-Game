using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraFunctionality : MonoBehaviour
{
    public InputActionReference PlayerActionMap;

    public Transform MainCamera;

    public Vector3 PlayerVelocity;

    public float FinalMoveSpeed;
    public float CurrentMoveSpeed;
    public float Incrimenter;
    public float RotationSpeed;

    public Transform PlayerOrientation;
    public Transform Player;
    public Transform PlayerObject;

    public Vector3 ViewDirection;

    public Vector3 TransformDirection;
    [SerializeField] protected Vector3 MoveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateToView();
        //TestView();
    }

    public void RotateToView()
    {
        ViewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        PlayerOrientation.forward = ViewDirection.normalized;

        MoveDirection = PlayerActionMap.action.ReadValue<Vector3>();
        MoveDirection = new Vector3(MoveDirection.z, 0, MoveDirection.x * -1);
        Vector3 InputDirection = PlayerOrientation.right * MoveDirection.x + PlayerOrientation.forward * MoveDirection.z;

        if (InputDirection != Vector3.zero)
        {
            PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, InputDirection.normalized, Time.deltaTime * RotationSpeed);

        }

    }

    public void TestView()
    {

        Vector3 viewDir = PlayerOrientation.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        PlayerOrientation.forward = viewDir.normalized;

        // roate player object

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;


        if (inputDir != Vector3.zero)
            PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, inputDir.normalized, Time.deltaTime * RotationSpeed);
    }



    protected IEnumerator SpeedTransition()
    {
        float TransitionTime = 0;
        float SpeedDifference = Mathf.Abs(FinalMoveSpeed - CurrentMoveSpeed);
        float InitialMoveSpeed = CurrentMoveSpeed;

        while (TransitionTime < SpeedDifference)
        {
            CurrentMoveSpeed = Mathf.Lerp(InitialMoveSpeed, FinalMoveSpeed, TransitionTime / SpeedDifference);
            TransitionTime += Time.deltaTime * Incrimenter;
            yield return null;
        }
        CurrentMoveSpeed = FinalMoveSpeed;
    }


}
