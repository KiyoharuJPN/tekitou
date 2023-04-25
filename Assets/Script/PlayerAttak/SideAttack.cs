using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SideAttack : MonoBehaviour
{
    public static void _SideAttack(PlayerController player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
        player.isSideAttack = true;
        player.animator.SetBool("IsSideAttack", player.isSideAttack);

        _Move(player, skill);

        Debug.Log("‹‡Ø‚èI");
    }

    static void _Move(PlayerController player, Skill skill)
    {
        float distance = skill.distance;
        while (distance > 0)
        {
            distance-- ;
            player.rb.AddForce(player.transform.right * skill.distance);
        }
    }

    static float _Ray(Rigidbody2D player, Vector2 playerSize, Vector2 vector, Skill skill)
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
        else if (hit.collider == null) { }
        {
            return 0;
        }
    }
}
