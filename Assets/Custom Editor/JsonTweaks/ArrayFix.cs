using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayFix
{
    public static string ToJson<T>(this T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>(array);
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;

        public Wrapper(T[] array)
        {
            this.array = array;
        }
    }
}

