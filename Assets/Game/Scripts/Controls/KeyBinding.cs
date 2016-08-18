using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

[Serializable]
public class KeyList
{
    [SerializeField]
    public List<Key> List = new List<Key>();
}

[Serializable]
public class BoundKeyDictionary : ICloneable
{
    [SerializeField]
    private List<KeyCode> _keys = new List<KeyCode>();
    public List<KeyCode> Keys { get { return _keys; } private set { _keys = value; } }

    [SerializeField]
    private List<KeyList> _values = new List<KeyList>();
    public List<KeyList> Values { get { return _values; } private set { _values = value; } } 

    public int Count { get { return Keys.Count; } }

    public object Clone()
    {
        var clone = new BoundKeyDictionary();
        for (int i = 0; i < Keys.Count; i++)
        {
            clone.Keys.Add(Keys[i]);
            clone.Values.Add(new KeyList());
            for (int j = 0; j < Values[i].List.Count; j++)
            {
                clone.Values[i].List.Add(Values[i].List[j]);
            }
        }
        return clone;
    }

    public KeyList this[KeyCode key]
    {
        get
        {
            int index = Keys.FindIndex(k => k == key);
            if (index >= 0 && index < Values.Count)
                return Values[index];
            else
                throw new IndexOutOfRangeException();
        }
        set
        {
            int index = Keys.FindIndex(k => k == key);
            if (index >= 0 && index < Values.Count)
                Values[index] = value;
            else
            {
                Keys.Add(key);
                Values.Add(value);
            }
        }
    }

    public bool TryGetValue(KeyCode key, out KeyList value)
    {
        value = null;
        int index = Keys.FindIndex(k => k == key);
        if(index >= 0 && index < Values.Count)
        {
            value = Values[index];
            return true;
        }

        return false;
    }

    public bool ContainsKey(KeyCode key)
    {
        return Keys.Contains(key);
    }

    public bool Add(KeyCode key, KeyList value)
    {
        if (Keys.Contains(key))
            return false;

        Keys.Add(key);
        Values.Add(value);
        return true;
    }

    public bool Remove(KeyCode key)
    {
        int index = Keys.FindIndex(k => k == key);
        bool removeSuccessful = Keys.Remove(key);
        if (removeSuccessful)
            Values.RemoveAt(index);

        return removeSuccessful;
    }
}

[CreateAssetMenu()]
public class KeyBinding : ScriptableObject
{
    [SerializeField]
    BoundKeyDictionary _boundKeys;

    public BoundKeyDictionary BoundKeys { get { return _boundKeys; } set { _boundKeys = value; } }

}
