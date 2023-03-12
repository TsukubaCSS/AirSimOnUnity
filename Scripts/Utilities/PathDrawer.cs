using PlasticGui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    [SerializeField]
    private string _filePath;
    [SerializeField]
    private LineRenderer _renderer;

    void Start()
    {
        var lines = File.ReadAllLines(_filePath);

        int n = lines.Length - 1;
        _renderer.positionCount = n;
        for (int i = 0; i < n; ++i)
        {
            var (_, position, _) = ParsePosition(lines[i + 1]);
            _renderer.SetPosition(i, position);
        }
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
