using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class FieldClass : MonoBehaviour
{
    public int number;
    public Vector3 coords;
    public string name1;

    void Start()
    {
        coords = transform.position;
    }

    public FieldClass(int number, string name)
    {
        this.number = number;
        this.name1 = name;
    }
}
