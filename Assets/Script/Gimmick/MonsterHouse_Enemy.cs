using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHouse_Enemy : MonoBehaviour
{
    public MonsterHouse monsterHouse;

    public void Destroy()
    {
        monsterHouse.EnemyListRemove(this.gameObject);
    }
}
