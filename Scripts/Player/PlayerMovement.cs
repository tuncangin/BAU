using System;
using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rB2D;
    Animator anim;

    [SerializeField] float speed = 2;
    private float defSpeed;
    [SerializeField] float Highspeed = 4;
    [SerializeField] float jumpForce = 100;
    [SerializeField] private float rayCastDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float enemyKillDistance;
    [SerializeField] private Color dieColor;
    [SerializeField] private Color SpeedyColor;


    private InputHandler _handler;
    private Color defColor;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool canMove;
    private bool canJump;
    
    private float horizontal;
    private float vertical;

    [SerializeField] private Transform raycaster;
    void Start()
    {
        GettingReferences();
    }

    private void GettingReferences() //Oyun başlangıcında yapılacak değişiklikler
    {
        defSpeed = speed;
        canMove = true;
        rB2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defColor = GetComponent<SpriteRenderer>().color;
        gameObject.SetActive(false);
    }

    public void SetDefaultSettings() //Her level başladığında çağırılacak, player objesi üstünde yapılacak değişiklikler
    {
        rB2D.velocity = Vector2.zero;
        transform.position = GameManager.instance.levels[GameManager.instance.levelNumber].playerPosition;
        GetComponent<SpriteRenderer>().color = defColor;
        GetComponent<CircleCollider2D>().enabled = true;
        canMove = true;
    }
    private bool RayCaster() //isGrounded boolean'ı için ray atımı
    {
        isGrounded = Physics2D.Raycast(raycaster.transform.position, Vector2.down, rayCastDistance, groundLayer);
        return isGrounded;
    }

    private void Update()
    {
        RayCaster();
        ReverseComparison();
        GetInput();
        AnimationController();

        if (isGrounded && !canJump)
        {
            canJump = true;
        }
    }
    
    void GetInput() //Klavye inputları alıyor
    {
        
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vertical = 1;
        }
        else if(Input.GetKeyUp(KeyCode.Space)) 
        {
            vertical = 0;
            canDoubleJump = true;
        }
        
        if (horizontal != 0)
        {
            anim.SetBool(TAGS.isRunning, true);
        }
        else
        {
            anim.SetBool(TAGS.isRunning, false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) //Çarpışma ile ilgili kararlar
    {
        if (transform.position.y - enemyKillDistance > other.gameObject.transform.position.y && other.gameObject.CompareTag(TAGS.Enemy))
        {
            other.gameObject.GetComponent<EnemyAI>().KillMe();
            rB2D.velocity = Vector2.zero;
            rB2D.AddForce(Vector2.up * jumpForce * 0.02f);
        }
        else
        {
            if (other.gameObject.CompareTag(TAGS.Enemy))
            {
                KillMe();
            }
        }

        if (other.gameObject.CompareTag(TAGS.Killer))
        {
            KillMe();
        }

        if (other.gameObject.CompareTag(TAGS.Finish))
        {
            GameManager.instance.LevelCompleted();
            canMove = false;
        }
    }
    
    private void KillMe() //Player öldüğü zaman çağırılan fonksiyon
    {
        GameManager.instance.LevelFailed();
        rB2D.velocity = Vector2.zero;
        anim.SetBool(TAGS.isJumping, false);
        anim.SetBool(TAGS.isRunning, false);
        rB2D.GetComponent<SpriteRenderer>().color = dieColor;
        GetComponent<CircleCollider2D>().enabled = false;
        rB2D.AddForce(Vector2.up * jumpForce * 1.5f * Time.fixedDeltaTime);
        canMove = false;
    }

    private void OnTriggerEnter2D(Collider2D other) //Collectible toplamak için trigger fonksiyonu
    {
        if(other.gameObject.CompareTag(TAGS.Collectible))
        {
            DestroyCollectible(other);
        }

        if (other.gameObject.CompareTag(TAGS.SpeedUp))
        {
            GetComponent<SpriteRenderer>().color = SpeedyColor;
            DestroyCollectible(other);
            SpeedUp();
        }
    }

    private void DestroyCollectible(Collider2D other) // collectible alındığında çalışan fonksiyon
    {
        UIManager.instance.Collected();
        other.gameObject.GetComponent<Collectible>().particle.SetActive(true);
        other.gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        Destroy(other.gameObject, 2);
    }

    private void SpeedUp() //player'ı collectible ile hızlandırıp 5 saniye sonra tekrar normal hızına çevirme
    {
        StartCoroutine(SpeedDown());
    }

    IEnumerator SpeedDown() // 5 saniye sonra geri eski haline dönmesini sağlayan coroutine
    {
        speed = Highspeed;
        yield return new WaitForSeconds(5);
        GetComponent<SpriteRenderer>().color = defColor;
        speed = defSpeed;
    }
    
    void FixedUpdate() //Fizik işlemleri olduğu için fixed update'e yazıldı
    {
        if (canMove)
        {
            playerMovement();
            Jump();
        }
    }

    void AnimationController()  //Objenin lokasyonuna göre animator düzenliyor
    {
        if (isGrounded)
        {
            anim.SetBool(TAGS.isJumping, false);
        }
        else
        {
            anim.SetBool(TAGS.isJumping, true);
        }
    }

    private void playerMovement() //Player'ın hareketinden sorumlu
    {
        if (horizontal != 0)
        {
            Vector2 force = new Vector2(horizontal * speed * Time.fixedDeltaTime, 0);
            rB2D.velocity = new Vector2(force.x, rB2D.velocity.y);
        }
        else
        {
            rB2D.velocity = new Vector2(0, rB2D.velocity.y);
        }
    }

    private void Jump() //Zıplamadan sorumlu, double jump mevcut
    {
        if (isGrounded && canJump)
        {
            if (vertical != 0)
            {
                //jump
                rB2D.AddForce(new Vector2(0, jumpForce * vertical * Time.fixedDeltaTime));
                anim.SetBool(TAGS.isJumping, true);
                vertical = 0;
            }
        }
        else
        {
            if (canDoubleJump && canJump)
            {
                if (vertical != 0)
                {
                    //double jump
                    rB2D.velocity = new Vector2(rB2D.velocity.x, 0);
                    rB2D.AddForce(new Vector2(0, jumpForce* 1.2f * vertical * Time.fixedDeltaTime));
                    anim.SetTrigger(TAGS.isDoubleJumping);
                    canDoubleJump = false;
                    canJump = false;
                }
            }
        }
    }
    
    
    void ReverseComparison() //Objenin yüzünün nereye bakacağına karar verip çevirme fonksiyonunu çağırıyor
    {
        if (rB2D.velocity.x > 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Reverse();
        }
        else if (rB2D.velocity.x < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Reverse();
        }
    }
    
    void Reverse()  //Objenin yüzünü doğru yöne çeviriyor
    {
        Vector3 charScale = transform.localScale;
        charScale.x *= -1;
        transform.localScale = charScale;
    }
    
}
