using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyListEntity : ScriptableObject
{
    public List<EnemyData> EnemyDataList = new List<EnemyData>();
}