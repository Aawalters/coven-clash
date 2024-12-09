using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Waypoint : MonoBehaviour
{

    [SerializeField] public Vector3[] Points;

    public Vector3 CurrentPos;

    private Vector3 _currentPos;
    private bool _gameStarted;

    void Start()
    {
        _gameStarted = true;
        _currentPos = transform.position;
    }

    public Vector3 GetWaypointPos(int index)
    {
        return CurrentPos + Points[index];
    }

    private void OnDrawGizmos()
    {
        if (!_gameStarted && transform.hasChanged)
        {
            _currentPos = transform.position;
        }

        for (int i = 0; i < Points.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Points[i] + _currentPos, 0.5f);

            if (i < Points.Length - 1)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(Points[i] + _currentPos, Points[i + 1] + _currentPos);
            }
        }
    }
}
