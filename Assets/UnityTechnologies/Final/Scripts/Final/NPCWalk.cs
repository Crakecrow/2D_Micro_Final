using UnityEngine;

public class NPCWalking : MonoBehaviour
{
    // ====== NPC MOVEMENT ========
    public float speed;           
    public float timeToChange;     
    public bool horizontal;        

    private Rigidbody2D rigidbody2d;    
    private SpriteRenderer spriteRenderer; 
    private float remainingTimeToChange;  
    private Vector2 direction;          

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        direction = horizontal ? Vector2.right : Vector2.down;

      
        remainingTimeToChange = timeToChange;
    }

    void Update()
    {
 
        rigidbody2d.MovePosition(rigidbody2d.position + direction * speed * Time.deltaTime);

       
        remainingTimeToChange -= Time.deltaTime;

        
        if (remainingTimeToChange <= 0)
        {
            remainingTimeToChange += timeToChange; 
            direction *= -1;                      
            spriteRenderer.flipX = !spriteRenderer.flipX; 
        }
    }
}