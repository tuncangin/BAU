using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private Transform Raycaster;
    [SerializeField] private Transform ObstacleController;
    [SerializeField] private float RayDistanceforGround;
    [SerializeField] private float RayDistanceforPlayerSearch;
    [SerializeField] private float RayDistanceforObstacles;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float enemySpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Color dieColor;
    
    private Color defColor;
    private Rigidbody2D rb;
    private Animator anim;
    

    private bool PointAReach,PointBReach;
    
    private bool shouldJump;
    private bool isFacingLeft;
    private bool isGrounded;
    private bool canAttack;
    private bool canMove;

    private void Start()
    {
        GetReferences();
    }

    private void GetReferences() //Level başladığında gerekli referansları almasını sağlıyor
    {
        Player = GameManager.instance.Player.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        canMove = true;
        defColor = GetComponent<SpriteRenderer>().color;
    }

    public void KillMe()  //Enemy üstüne basıldığında çağırılıyor
    {
        canMove = false;
        GetComponent<SpriteRenderer>().color = dieColor;
        anim.SetBool(TAGS.isRunning,false);
        anim.SetBool(TAGS.isJumping, false);
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce * .07f);
        GetComponent<CircleCollider2D>().enabled = false;
    }
    
    private void GroundCheck() //Enemy GroundCheck yapıyor
    {
        isGrounded = Physics2D.Raycast(Raycaster.transform.position, Vector2.down, RayDistanceforGround, groundLayer);
        anim.SetBool(TAGS.isJumping,!isGrounded);
    }

    private void PlayerCheck() //Enemy, player arıyor
    {
        canAttack = Physics2D.Raycast(Raycaster.transform.position, Player.position - transform.position, RayDistanceforPlayerSearch, playerLayer);
    }

    private void ObstacleCheck() //Enemy engel arıyor
    {
        if (!isFacingLeft)
        {
            shouldJump = Physics2D.Raycast(ObstacleController.transform.position, ObstacleController.transform.right, RayDistanceforObstacles, groundLayer);
        }
        else
        {
            shouldJump = Physics2D.Raycast(ObstacleController.transform.position, -ObstacleController.transform.right, RayDistanceforObstacles, groundLayer);
        }
    }

    private void Update()
    {
        if (canMove)
        {
            GroundCheck();
            PlayerCheck();
            ObstacleCheck();
            EnemyMovement();
            ReverseComparison();
        }
    }

    private void EnemyMovement() //enemy hareketleri, atak halinde ya da patrol atarken yapacağı aksiyon
    {
        if (!canAttack)
        {
            //A noktasına git gidince b noktasına git
            PatrolMovement();
        }
        else
        {
            //Player a doğru git ve saldır
            AttackToPlayer();
        }
    }

    private void AttackToPlayer()  /*Player gördüyse atağa geçişi, hızı artar, aradaki mesafe verilen değeri koruduğu sürece engel gördükçe zıplayarak player'ı takip eder.
    Buna sonradan ekstra verilecek raycasterlarla altının boşluk olup olmadığını kontrol edip yan bloklara zıplamasını da verebiliriz.*/
    {
        if (!shouldJump)
        {
            if (transform.position.x > Player.position.x)
            {
                anim.SetBool(TAGS.isRunning,true);
                Vector2 force = new Vector2(-1 * enemySpeed*1.2f * Time.fixedDeltaTime, 0);
                rb.velocity = new Vector2(force.x, rb.velocity.y);
            }else if (transform.position.x < Player.position.x)
            {
                anim.SetBool(TAGS.isRunning, true);
                Vector2 force = new Vector2(1 * enemySpeed *1.2f* Time.fixedDeltaTime, 0);
                rb.velocity = new Vector2(force.x, rb.velocity.y);
            }
        }
        else
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0, jumpForce * Time.fixedDeltaTime));
            }
            
        }
    }
    
    
    private void PatrolMovement()  //Patrol hareketi, AI içerir, bir engele yaklaştığında zıplamasını ve önceden ayarlı patrol alanları arasında gezmesini sağlar.
    {
        if (!shouldJump)
        {
            if(!PointAReach)
            {
                if (transform.position.x != pointA.position.x)
                {
                    anim.SetBool(TAGS.isRunning,true);
                    Vector2 force = new Vector2(-1 * enemySpeed * Time.fixedDeltaTime, 0);
                    rb.velocity = new Vector2(force.x, rb.velocity.y);
                }
                if (transform.position.x <= pointA.position.x)
                {
                    PointAReach = true;
                    PointBReach = false;
                }
            
            }
            else if(!PointBReach)
            {
                if (transform.position.x != pointB.position.x)
                {
                    anim.SetBool(TAGS.isRunning,true);
                    Vector2 force = new Vector2(1 * enemySpeed * Time.fixedDeltaTime, 0);
                    rb.velocity = new Vector2(force.x, rb.velocity.y);
                }
                if (transform.position.x >= pointB.position.x)
                {
                    PointBReach = true;
                    PointAReach = false;
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0, jumpForce * Time.fixedDeltaTime));
            }
        }
        
    }

 
    void ReverseComparison()  //Yüzünü döneceği yöne karar verme
    {
        if (rb.velocity.x < 0 && !isFacingLeft)
        {
            isFacingLeft = !isFacingLeft;
            Reverse();
        }
        else if (rb.velocity.x > 0 && isFacingLeft)
        {
            isFacingLeft = !isFacingLeft;
            Reverse();
        }
    }
    
    void Reverse()  //Yüzünü döndürme
    {
        Vector3 charScale = transform.localScale;
        charScale.x *= -1;
        transform.localScale = charScale;
    }
}
