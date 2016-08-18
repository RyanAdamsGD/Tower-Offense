using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(KeyBinding))]
public class KeyBindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        KeyBinding tar = target as KeyBinding;

        //make a copy of the original object just for the safety of the for loops
        BoundKeyDictionary boundKeys = tar.BoundKeys;
        if(boundKeys == null)
        {
            Debug.Log("Created bound keys.");
            boundKeys = new BoundKeyDictionary();
        }

        //make a copy to keep all of the changes that will happen
        var updatedDictionary = boundKeys.Clone() as BoundKeyDictionary;

        //display the + for adding new entries to the list
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Keybindings");
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("+"))
        {
            AddNew(updatedDictionary);
        }
        EditorGUILayout.EndHorizontal();
        //loop over all values in the dictionary and show the popups for them
        EditorGUI.indentLevel++;
        foreach (var keyCode in boundKeys.Keys)
        {
            foreach (var key in boundKeys[keyCode].List)
            {
                EditorGUILayout.BeginHorizontal();
                KeyCode newKeyCode = (KeyCode)EditorGUILayout.EnumPopup(keyCode);
                Key newKey = (Key)EditorGUILayout.EnumPopup(key);
                //Button for removing items
                if (GUILayout.Button("X"))
                {
                    Remove(keyCode, key, updatedDictionary);
                }
                //apply any changes that were made
                if (newKeyCode != keyCode || newKey != key)
                {
                    Remove(keyCode, key, updatedDictionary);
                    Add(newKeyCode, newKey, updatedDictionary);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUI.indentLevel--;

        //update the original object if needed
        if(EditorGUI.EndChangeCheck())
        {
            //new unity call for storing undos
            Undo.RecordObject(target, "Keybinding Change");
            tar.BoundKeys = updatedDictionary;
        }
    }

    private void AddNew(BoundKeyDictionary updatedDictionary)
    {
        int value = 0;
        while ((Key)value < Key._Count)
        {
            if(Add((KeyCode)1,(Key)value++,updatedDictionary))
            {
                return;
            }
        }
        Debug.LogError("Unable to add additional values, due to Key 0 being mapped to everything! Adjust the current bindings and try again.");
    }


    /// <summary>
    /// Returns False if the value is already in the list
    /// </summary>
    private bool Add(KeyCode keyCode, Key key, BoundKeyDictionary updatedDictionary)
    {
        KeyList value = null;
        //add a new hashset if the key is not already in the dictionary
        if (!updatedDictionary.ContainsKey(keyCode))
            updatedDictionary.Add(keyCode, new KeyList());
        //check if there is already a hashset in the dictionary, add one if there isn't
        else if (!updatedDictionary.TryGetValue(keyCode, out value) || value == null)
            updatedDictionary[keyCode] = new KeyList();

        //check if the value is already in the hashset
        bool alreadyExists = updatedDictionary[keyCode].List.Contains(key);
        if(!alreadyExists)
            updatedDictionary[keyCode].List.Add(key);

        return !alreadyExists;
    }

    private void Remove(KeyCode keyCode, Key key, BoundKeyDictionary updatedDictionary)
    {
        KeyList value = null;
        //check if there is an entry stored for this keycode
        if (updatedDictionary.ContainsKey(keyCode) && updatedDictionary.TryGetValue(keyCode, out value) && value != null)
        {
            //remove the entry from the hashset
            updatedDictionary[keyCode].List.Remove(key);
            //remove the hashset if it is empty
            if (updatedDictionary[keyCode].List.Count == 0)
                updatedDictionary.Remove(keyCode);
        }
    }
}