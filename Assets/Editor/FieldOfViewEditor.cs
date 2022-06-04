using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//enemyai ĳ���Ϳ� �þ߰� �����ϱ� 
[CustomEditor(typeof(EnemyAiTutorial))]
public class FieldOfViewEditor : Editor
{

    private void OnSceneGUI()
    {
        EnemyAiTutorial fov = (EnemyAiTutorial)target;
        //�þ� ����
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);
        //���� ����
        Handles.color = Color.black;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.attackRadius);

        Vector3 viewAngel01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2); // enemy �þ� ���� ������ ����
        Vector3 viewAngel02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);


        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngel01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngel02 * fov.radius);

        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngel01 * fov.attackRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngel02 * fov.attackRadius);

        if (fov.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }

        if (fov.canAttackPlayer)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegress)
    {
        angleInDegress += eulerY;

        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }
}
