using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Night AI Data")]
public class NightAIData : ScriptableObject
{
    public int night; //Night Number
    public float powerDrain;
    public List<AnimatronicAI> animatronics;
}

[System.Serializable]
public struct AnimatronicAI
{
    public Animatronic animatronic;
    public int aiLevel;
}