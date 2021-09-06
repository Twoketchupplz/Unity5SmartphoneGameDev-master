using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator ani;
    void Awake()
    {// 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    void Update() { //단발적인 입력
    //GetButton : 누르고 있는 상태, GetButtonDown : 버튼이 눌릴 때, GetButtonUp : 버튼이 떨어질때

        //Jump
        if (Input.GetButtonDown("Jump") && !ani.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            ani.SetBool("isJumping", true);
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        
        {
            // nomalized : 벡터 크기를 1로 만든 상태 단위벡터, 여기선 단위벡터 * 0.5만큼 속도로 계속 움직인다. 공기저항이 없다면..
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) //횡이동 속도 단위가 0이라면
        {
            ani.SetBool("isWalking", false);
        }
        else { 
            ani.SetBool("isWalking", true);
        }

    }

    void FixedUpdate() // 지속적인 키입력 fp사이에 키입력이 씹힐 수 있음
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal")*5;

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); //right = (1,0)을 의미한다 정방향 left는 (-1,0) 누르는 방향의 반대로 움직인다.
        //impulse : 순간적인 힘을 가함 그외에도 force acceleration velocitychange 가 있다.

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);


        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            //DrawRay 에디터 상에서만 Ray를 보여줌
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            // Ray에 닿은 오브젝트
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            //RayCast : 오브젝트를 검색하기 위해 Ray를 쏘는 방식
            if (rayHit.collider != null) //Ray를 쐈는데 null이 아니다 즉 collider가 있다.
            {
                if (rayHit.distance < 0.5f) //플레이어 크기가 1인데 중간부터 재면 0.5 이하이다.
                {
                    ani.SetBool("isJumping", false);
                    //Debug.Log(rayHit.collider.name); //이거만 쓰면 player collider에 먼저 걸린다. Layer - ignore rayCast
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) //충돌
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Attack 속도가 음의 방향이면서 Y위치가 collision Y보다 높을때
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }else //Damaged
                OnDamaged(collision.transform.position);
        }
    }
    void OnDamaged(Vector2 targetPos) {
        gameObject.layer = 11;
        
        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction , 1)*7, ForceMode2D.Impulse);

        // Animation
        ani.SetTrigger("doDamaged");
        Invoke("OffDamaged", 2);

    }
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //Point

        //Enemy Die
        Enemymove enemyMove = enemy.GetComponent<Enemymove>();
        enemyMove.OnDamaged();
    }
}

// 현재 밟으면 멈춘다 에러
// BE2 초반부