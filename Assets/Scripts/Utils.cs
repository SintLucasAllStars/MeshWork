using System;
using System.Collections.Generic;
using System.Linq;

public class Utils
{
    public static float Map(float value, float valueMin, float valueMax, float resultMin, float resultMax)
    {
        if (resultMin == resultMax) return resultMin;
        if (valueMin == valueMax) return resultMax;
        return resultMin + (value-valueMin)*(resultMax-resultMin)/(valueMax-valueMin);
    }
    
    public static IEnumerable<T> GetEnumValues<T>() {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}