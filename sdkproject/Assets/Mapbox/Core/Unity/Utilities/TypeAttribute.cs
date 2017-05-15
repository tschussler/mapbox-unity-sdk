using UnityEngine;
using System.Collections;

public class TypeAttribute : PropertyAttribute
{
    public System.Type Type;

    public TypeAttribute(System.Type t)
    {
        Type = t;
    }
}
