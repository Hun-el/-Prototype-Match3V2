using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gem" , menuName = "Gem")]
public class Gems : ScriptableObject
{
    public Sprite gemArt;
    public Color color;
    public enum GemType {blue,green,purple,red,yellow,bomb}
    public GemType type;
    public GameObject destroyEffect;
    public GameObject bombEffect;
    public int blastSize;
}
