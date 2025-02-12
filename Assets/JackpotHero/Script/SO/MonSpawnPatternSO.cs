using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MonSpawnPattern", menuName = "SO/MonSpawnPattern")]
public class MonSpawnPattern : ScriptableObject
{
    public int SpawnPatternID;
    public string[] SpawnMonsterName;
    public float RewardEXPPoint;
    public float VariationEXPPoint;
}