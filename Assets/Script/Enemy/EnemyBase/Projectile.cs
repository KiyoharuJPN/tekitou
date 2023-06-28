using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public virtual void EnemyStop()
    {
        //isPlayerExAttack = true;
        //enemyRb.velocity = Vector2.zero;
        //if (animator != null)
        //{
        //    animator.speed = 0;
        //}
    }

    //ïKéEãZÇ™ìñÇΩÇ¡ÇƒÇ¢ÇΩèÍçá
    public virtual void PlaeyrExAttack_HitEnemyEnd(float powar)
    {
        //if (animator != null)
        //{
        //    animator.speed = 1;
        //}
        //isPlayerExAttack = false;
        //Damage(powar);
    }

    //ìñÇΩÇ¡ÇƒÇ¢Ç»Ç¢èÍçá
    public virtual void Stop_End()
    {
        //isPlayerExAttack = false;
        //if (animator != null)
        //{
        //    animator.speed = 1;
        //}
        //isPlayerExAttack = false;
    }
}
