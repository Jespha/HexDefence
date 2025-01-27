using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFunctions : MonoBehaviour
{
    public Vector3[] GetSourroundingHexCoords(Vector3 center, float radius)
    {
        Vector3[] _neigbours = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i + 30; // Add an offset of 30 degrees
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 position = new Vector3(
                center.x + radius * Mathf.Cos(angle_rad),
                center.y,
                center.z + radius * Mathf.Sin(angle_rad)
            );
            _neigbours[i] = position;
        }

        return _neigbours;
    }
}
