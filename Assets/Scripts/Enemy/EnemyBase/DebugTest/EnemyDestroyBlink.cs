using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyDestroyBlink")]
public class EnemyDestroyBlink : ScriptableObject
{
    public DestroyBlink DestroyBlinkSpeed = new DestroyBlink();
}