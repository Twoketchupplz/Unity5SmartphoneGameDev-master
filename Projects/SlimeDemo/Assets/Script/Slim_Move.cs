using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim_Move : MonoBehaviour {
    public float maxSpeed;
    public float jumpPower;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator ani;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    void Update() {
        //sprite direction
        if (Input.GetButton("Horizontal")) {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == 1;
        }

        //jump and jumping animation
        if (Input.GetButtonDown("Jump") && !ani.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            ani.SetBool("isJumping", true);
        }

        //stop speed
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //walking animation
        if (Mathf.Abs(rigid.velocity.x) == 0) {
            ani.SetBool("Idle", true);
            ani.SetBool("isWalking", false);
        } else {
            ani.SetBool("Idle", false);
            ani.SetBool("isWalking", true);
        }
        
    }

    void FixedUpdate() {
        //Move Speed
        float speed = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } else if (rigid.velocity.x < maxSpeed * (-1) ) {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //landing platform
        if (rigid.velocity.y <= 0) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            //RaycastHit2D raycastHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));
            RaycastHit2D raycastHit = Physics2D.Raycast(rigid.position, Vector3.down); //뭐가 있던 있다 없다로 판단
            if (raycastHit.collider != null) {
                if (raycastHit.distance < 0.5f) {
                    ani.SetBool("isJumping", false);
                }
            }
        }


    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y <= 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            }
        }
    }

    void OnAttack(Transform target) {
        Enemy_Move enemyMove = target.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();
    }
    void OnDamaged() {

    }
}
