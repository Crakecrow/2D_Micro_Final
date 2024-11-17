using UnityEngine;

/// <summary>
/// Will handle giving health to the character when they enter the trigger.
/// </summary>
public class HealthCollectible : MonoBehaviour 
{
    public ParticleSystem collectHealth;
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(1);
                if (collectHealth != null)
            {
                Instantiate(collectHealth, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
