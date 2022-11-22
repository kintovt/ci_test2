using System;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query)
    {
        if (parent.childCount == 0)
        {
            return null;
        }

        Transform result = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (query(child))
            {
                return child;
            }
            result = FirstChildOrDefault(child, query);
            if(result!=null)
                break;
        }

        return result;
    }
}