using UnityEngine;

namespace Script
{
    public class PlayerMovement : MonoBehaviour {
        public float maxSpeed;
        public float jumpPower;

        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigid;
        private Animator _ani;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Jump = Animator.StringToHash("isJumping");

        void Awake() {
            _rigid = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _ani = GetComponent<Animator>();
        }

        void Update() { //컴퓨터에 따라 다른 호출주기를 가질 수 있다. 성능차이
            //sprite direction
            if (Input.GetButton("Horizontal")) {
                _spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            }

            // jump and jumping animation
            if (Input.GetButtonDown("Jump") && !_ani.GetBool(Jump)) {
                _rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                _ani.SetBool(Jump, true);
            }

            //stop speed
            if (Input.GetButtonUp("Horizontal")) {
                _rigid.velocity = new Vector2(0, _rigid.velocity.y);
            }

            // walking animation
             if (Mathf.Abs(_rigid.velocity.x) == 0) {
                 _ani.SetBool(Idle, true);
                 _ani.SetBool(Walk, false);
             } else {
                 _ani.SetBool(Idle, false);
                 _ani.SetBool(Walk, true);
             }
        
        }

        void FixedUpdate() { //고정된 호출주기를 가지고 있다.(리소스나 환경의 영향을 받지 않는다.)
            //Move Speed
            float speed = Input.GetAxisRaw("Horizontal");
            _rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);

            if (_rigid.velocity.x > maxSpeed){
                _rigid.velocity = new Vector2(maxSpeed, _rigid.velocity.y); //y축 방향을 0으로 잡으면 체공 중 멈추게 된다.
            } else if (_rigid.velocity.x < maxSpeed * (-1) ) {
                _rigid.velocity = new Vector2(maxSpeed * (-1), _rigid.velocity.y);
            }

            //landing platform
            /*if (_rigid.velocity.y <= 0) {
                Debug.DrawRay(_rigid.position, Vector3.down, new Color(0, 1, 0));
                //RaycastHit2D raycastHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));
                RaycastHit2D raycastHit = Physics2D.Raycast(_rigid.position, Vector3.down); //뭐가 있던 있다 없다로 판단
                if (raycastHit.collider != null) {
                    if (raycastHit.distance < 0.5f) {
                        _ani.SetBool("isJumping", false);
                    }
                }
            }*/


        }

        /*private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y <= 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            }
        }
    }*/

        /*void OnAttack(Transform target) {
        Enemy_Move enemyMove = target.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();
    }
    void OnDamaged() {
        

    }*/
    }
}
