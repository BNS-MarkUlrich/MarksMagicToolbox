using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{

    [SerializeField]
    private List<SerializableTuple<TKey, TValue>> dictionaryEntries = new();

    public void OnBeforeSerialize() {}

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < dictionaryEntries.Count; i++)
        {
            if (this.ContainsKey(dictionaryEntries[i].Item1))
            {
                Debug.LogError($"Duplicate key found in dictionary: {dictionaryEntries[i].Item1}");
                //dictionaryEntries.RemoveAt(i);
                continue;
            }
            this.Add(dictionaryEntries[i].Item1, dictionaryEntries[i].Item2);
        }
    }    
}
