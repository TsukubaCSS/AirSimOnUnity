using PlasticGui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PathDrawer : MonoBehaviour
{
    [SerializeField]
    private string _filePath;
    [SerializeField]
    private LineRenderer _renderer;
    [SerializeField]
    private float _drawOffsetSeconds;

    void Start()
    {
        var lines = File.ReadAllLines(_filePath);

        var positions = new List<Vector3>();
        int n = lines.Length - 1;
        for (int i = 0; i < n; ++i)
        {
            var (time, position, _) = ParsePosition(lines[i + 1]);
            if (time >= _drawOffsetSeconds)
            {
                positions.Add(position);
            }
        }

        _renderer.positionCount = positions.Count;
        _renderer.SetPositions(positions.ToArray());

    }

    private (float, Vector3, Vector3) ParsePosition(string line)
    {
        var tokens = line.Split();
        var timestamp = float.Parse(tokens[0]);
        var position = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
        var angles = new Vector3(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]));
        return (timestamp, position, angles);
    }
}
