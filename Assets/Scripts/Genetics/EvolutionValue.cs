using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public struct EvolutionValue
{
    public object value;
    public EvolutionValueType type;

    public EvolutionValue(object _value, EvolutionValueType _type)
    {
        value = _value;
        type = _type;
    }

    public T GetEvolutionValue<T>()
    {
        return (T)value;
    }
}

//public static class Evo
//{
//    public static readonly Dictionary<EvolutionValueType, Type> _Types = new Dictionary<EvolutionValueType, Type>
//    {
//        { EvolutionValueType.EvoInt, typeof(int) },
//        { EvolutionValueType.EvoFloat, typeof(float) },
//        { EvolutionValueType.EvoChar, typeof(char) }
//    };
//}