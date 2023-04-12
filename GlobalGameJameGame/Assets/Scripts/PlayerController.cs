using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //fields
    public InputAction playerControls;
    private Vector2 direction = Vector2.zero;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashLength;
    private bool isDashing;
    private bool canDash;
   
    [SerializeField]
    private float dashCooldown;
    public new Rigidbody2D rigidbody;
    private bool canJump = true;

    private Vector2 respawnPoint;

    //Animation move
    private bool isRunning = false;
    [SerializeField]
    private Animator animator;
    private float horizontal;//playerPosPre, 
    public GameObject obj;

    //EffectSound
    public AudioClip jump, dash;
    //float Mtime = 0.5f;
    public AudioSource WalkSE;

    public bool CanJump
    {
        get { return canJump; }
        set { canJump = value; }
    }

    public Vector2 RespawnPoint
    {
        get { return respawnPoint; }
        set { respawnPoint = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Animation move
        //playerPosPre = 0f;

        CanJump = true;
        isDashing = false;
        canDash = true;

        respawnPoint = new Vector2(rigidbody.position.x, rigidbody.position.y);

    }

    private void FixedUpdate()
    {
        //updates player's horizontal velocity
        rigidbody.velocity = new Vector2((direction.x * speed * Time.deltaTime), rigidbody.velocity.y);
       

        //Debug.Log(rigidbody.velocity.x);
    }
    // Update is called once per frame
    void Update()
    {
        direction = playerControls.ReadValue<Vector2>();


        //Animation move
        horizontal = direction.x;
        isRunning = horizontal != 0f;
        if (isRunning)
        {
            if (canJump)
            {
                if (!WalkSE.isPlaying)
                {
                    WalkSE.Play();
                }
                //test
                /*Mtime -= Time.deltaTime;
                if (Mtime < 0)
                {
                    EffectSoundSource.instance.PlaySE(walking);
                    Mtime = 0.5f;
                }*/
            }
            else
            {
                if(WalkSE.isPlaying) WalkSE.Stop();
            }
            Vector3 scale = gameObject.transform.localScale;
            if (horizontal < 0 && scale.x > 0 || horizontal > 0 && scale.x < 0)
            {
                scale.x *= -1;
            }
            gameObject.transform.localScale = scale;
        }
        else { if(WalkSE.isPlaying) WalkSE.Stop(); }
        animator.SetBool("IsWalk", isRunning);
        animator.SetBool("IsJump", !canJump);
        animator.SetFloat("ColorLevel", GetComponentInChildren<DarkController>().lightMeterUI.fillAmount);
        Debug.Log(CanJump);
        //playerPosPre = transform.position.x;
    }

    private void OnExitGame(InputValue value)
    {
        if (obj.activeSelf) { obj.SetActive(false); }
        if (!obj.activeSelf) { obj.SetActive(true); }
    }

    private void OnJump(InputValue value)
    {

        if (canJump)
        {
            EffectSoundSource.instance.PlaySE(jump);
            rigidbody.AddForce(new Vector2(0, jumpHeight));
            //canJump = false;
        }

    }
    private void OnDash(InputValue value)
    {
        direction = playerControls.ReadValue<Vector2>();
       
        /*
        if(direction.x == 0)
        {
            rigidbody.AddForce(Vector2.right * dashSpeed);
        }
        else
        {
            rigidbody.AddForce(direction * dashSpeed);
        }
        */

        if (canDash)
        {
            EffectSoundSource.instance.PlaySE(dash);
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        
            isDashing = true;
            canDash = false;

            float originalGravity = rigidbody.gravityScale;
            //keeps the player's vertical height constant while dashing
            rigidbody.gravityScale = 0.0f;

        //if the player has no horizontal direction dashes them to the right
            if (direction.x == 0)
            {
                rigidbody.AddForce(new Vector2(dashSpeed, 0));
               
            }
            else
            { 
               rigidbody.AddForce(new Vector2((direction.x * dashSpeed), 0.0f));
            }
            

            //waits until the end of the player's dash
            yield return new WaitForSeconds(dashLength);
            //resets gravity so player can fall after dashing
            rigidbody.gravityScale = originalGravity;
            isDashing = false;

            //cooldown
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
    }



    //Jump adjustment
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanJump = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CanJump = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CanJump = false;
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
