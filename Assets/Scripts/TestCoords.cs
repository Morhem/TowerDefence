using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoords : MonoBehaviour
{
    public MeshFilter filter;
    public Vector3 ScreenCoords;
    public Vector3 WorldCoords;
    Plane plane;
    void Start()
    {
        var filter = GameObject.Find("Plane").GetComponent<MeshFilter>();
        Vector3 normal = Vector3.zero;
        if (filter && filter.mesh.normals.Length > 0)
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);
        plane = new Plane(normal, transform.position);
    }

    void Update()
    {

        var ray = Camera.main.ScreenPointToRay(ScreenCoords);

        float enter;
        var test = plane.Raycast(ray, out enter);

        var final = ray.GetPoint(enter);
        WorldCoords = final;
        transform.position = WorldCoords;
    }
}
