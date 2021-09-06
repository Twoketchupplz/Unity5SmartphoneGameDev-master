using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    CapsuleCollider2D collider;
    Animator ani;

    public int vec; //행동지표

    // Start is called before the first frame update
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        ani = GetComponent<Animator>();

        Invoke("Think", 4);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(vec, rigid.velocity.y);

        //platform check
        Vector2 frontVec = new Vector2(rigid.position.x + vec * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D raycastHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        
        if(raycastHit.collider == null) {
            Debug.Log("몬스터 : 앞에 땅이 없습니다.");
            Turn();
        }
    }

    void Think() {
        //Move Speed and direction
        vec = Random.Range(-1, 2);
        Debug.Log("Monster : Next Move " + vec);

        //animation flipX,
        ani.SetInteger("WalkSpeed", vec);

        if (vec != 0) {
            spriteRenderer.flipX = vec == 1;
        }

        float thinkTime = Random.Range(1, 5);

        Invoke("Think", thinkTime);
        
    }

    void Turn() {
        vec *= -1;
        if (vec != 0) {
            spriteRenderer.flipX = vec == 1;
        }
    }

    public void OnDamaged() {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        collider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("Deactive", 3);
    }

    void Deactive() {
        gameObject.SetActive(false);
    }
}
