using System;

public interface IProgress
{
    event Action<ProgressChangedArgs> OnProgressChanged;
}

public struct ProgressChangedArgs
{
    public float maxValue;
    public float minValue;
    public float currentValue;
    public float progressNormalized;
}