using UnityEngine;

public class Slowdown : MonoBehaviour 
{
    public float slowDownFactor = 0.5f; // halving ruby's Speed
    private float originalSpeed; // Ruby chosen speed in inspector

//======================== Trigger event code to slow down speed==================    
	void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        
        if (controller != null)
        {
            originalSpeed = controller.speed; 
            controller.speed *= slowDownFactor; // this will slow down the player on trigger
        }
    }

//==================Trigger Event exit to return speed to normal===================    
	void OnTriggerExit2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        
        if (controller != null)
        {

            controller.speed = originalSpeed; 
        }
    }
}
