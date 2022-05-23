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
