using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Iaikiri
{
    public static void _Iaikiri(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.Skill1);
        float distance;

        if (player.transform.localScale.x <= 0)
        {
            distance = _Ray(player, -new Vector2(2,0), -player.transform.right, skill);

            if (distance != 0)
            {
                player.transform.Translate(-Vector3.right * distance);
                Debug.Log("ステージがあった");
            }
            else
            {
                player.transform.Translate(-Vector3.right * skill.distance);
                Debug.Log("ステージがなかった");
            }
        }
        else
        {
            distance = _Ray(player, new Vector2(2, 0), player.transform.right,skill);
            if (distance != 0)
            {
                player.transform.Translate(Vector3.right * distance);
                Debug.Log("ステージがあった");
            }
            else
            {
                player.transform.Translate(Vector3.right * skill.distance);
                Debug.Log("ステージがなかった");
            }
        }
        Debug.Log("居合切り！");
    }

    static float _Ray(Rigidbody2D player,Vector2 playerSize, Vector2 vector, Skill skill)
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position + playerSize, vector, skill.distance);
        Debug.DrawLine(player.position + playerSize, player.position + (vector * 10), Color.red, 1f);
        if (hit.collider != null)
        {
            float distance = Vector2.Distance(player.position, hit.collider.transform.position);
            Debug.Log("Distance to wall: " + distance);
            if (distance < 0.5f) return 0.2f;
            return distance;
        }
        else if(hit.collider == null) { }
        {
            return 0;
        }
    }
}
