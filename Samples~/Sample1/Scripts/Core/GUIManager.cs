using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEditor;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;
    
    public Canvas canvas;
    
    private void Awake()
    {
        Instance = this;
    }

    public void ShowPad(string padName)
    {
        Type type = Type.GetType($"Logic.{padName}");
        if(type==null) return;
        var pad = Activator.CreateInstance(type) as GUIPadBase;
        if(pad==null) return;
        var name = type.Name;
        var go = Resources.Load<GameObject>(name);
        GameObject padGo = Instantiate(go,canvas.transform);
        pad.BindRootNode(padGo);
        pad.OnAwake();
    }

    public void ShowPad<T>() where T:GUIPadBase,new()
    {
        Type type = typeof(T);
        var name = type.Name;
        var go = Resources.Load<GameObject>(name);
        GameObject padGo = Instantiate(go,canvas.transform);
        var t = new T();
        t.BindRootNode(padGo);
        t.OnAwake();
    }
}
