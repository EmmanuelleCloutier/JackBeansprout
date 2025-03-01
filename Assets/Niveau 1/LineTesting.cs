using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTesting : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private LineControl line;

    private void Start()
    {
        line.SetUpLine(points);
    }
}
