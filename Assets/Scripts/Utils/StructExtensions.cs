using System;
using System.Reflection;

public static class StructExtensions
{
    /// <summary>
    /// Sets the value of a field in a struct. Tip: use nameof() to get the field name.
    /// </summary>
    /// <typeparam name="T">The type of the struct.</typeparam>
    /// <typeparam name="U">The type of the field.</typeparam>
    /// <param name="target">The reference to the struct.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidCastException">Thrown if the field is not of type U.</exception>
    /// <exception cref="ArgumentException">Thrown if the field is not found in the struct.</exception>
    public static void SetField<T, U>(this ref T target, string fieldName, U value) where T : struct
    {
        FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            if (fieldInfo.FieldType == typeof(U))
                fieldInfo.SetValueDirect(__makeref(target), value);
            else
                throw new InvalidCastException($"Field '{fieldName}' is not of type '{typeof(U)}'.");
        }
        else
        {
            throw new ArgumentException($"Field '{fieldName}' not found in struct '{typeof(T)}'.");
        }
    }
}
