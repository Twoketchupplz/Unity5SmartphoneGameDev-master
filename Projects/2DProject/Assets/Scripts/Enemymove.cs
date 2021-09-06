using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemymove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator ani;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;

    public int nextMove; // 행동지표를 결정할 변수


    void Awake() //private 를 붙이면 작동을 안한다.. 엔진에게 공개를 안하는듯
    {
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 4);
    }


    void FixedUpdate()
    {   //Move 항상 Y축 속도는 0이아니라 현재 Y의 속도로 한다.
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y); //자신의 앞
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //RayCast : 오브젝트를 검색하기 위해 Ray를 쏘는 방식
        if (rayHit.collider == null) //Ray를 쐈는데 null이 아니다 즉 collider가 있다.
        {
            Debug.Log("Warning");
            Turn();
        }

    }

    void Think() //재귀 함수
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        ani.SetInteger("WalkSpeed", nextMove);
        
        //Flip Sprite
        if(nextMove != 0)
        spriteRenderer.flipX = nextMove == 1; //nextMove 가 체크 되면 실행

        //Recursive
        float nextThinkTime = Random.Range(2, 5);
        Invoke("Think", nextThinkTime);

    }
    void Turn()
    {
        nextMove *= -1;
        CancelInvoke(); //현재 작동 중인 모든 Invoke를 멈추는 함수
        spriteRenderer.flipX = nextMove == 1; //nextMove 가 체크 되면 실행

        Invoke("Think", 4);

    }

    public void OnDamaged() //PlayerMove에서 참조
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        collider.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
