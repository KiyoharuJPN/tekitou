using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyDebugTest")]
public class EnemyDebugTest : ScriptableObject
{
    public DebugTest test = new DebugTest();
}