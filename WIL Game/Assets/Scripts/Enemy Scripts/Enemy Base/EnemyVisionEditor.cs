using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyVision))]
public class EnemyVisionEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyVision EnemyVision = (EnemyVision)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(EnemyVision.transform.position, Vector3.up, Vector3.forward, 360, EnemyVision.Radius);

        Vector3 ViewAngle01 = DirectionFromAngle(EnemyVision.transform.eulerAngles.y, -EnemyVision.Angle / 2);
        Vector3 ViewAngle02 = DirectionFromAngle(EnemyVision.transform.eulerAngles.y, EnemyVision.Angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(EnemyVision.transform.position, EnemyVision.transform.position + ViewAngle01 * EnemyVision.Radius);
        Handles.DrawLine(EnemyVision.transform.position, EnemyVision.transform.position + ViewAngle02 * EnemyVision.Radius);

        if (EnemyVision.CanSeePlayer)
        {
            Handles.color = Color.green;
            //Handles.DrawLine(EnemyVision.transform.position, EnemyVision.PlayerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float EulerY, float AngleInDegrees)
    {
        AngleInDegrees += EulerY;

        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }
}
