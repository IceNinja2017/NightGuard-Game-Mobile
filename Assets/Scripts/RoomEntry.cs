using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Roomvalue
{
    public string animationName;
    public Transform trans;
}

[System.Serializable]
public class RoomEntry : MonoBehaviour
{
    public string key;
    public Roomvalue value;
}