using UnityEngine;
/// <summary>
/// this script manages my extinguisher projectiles, by assigning their type. giving them a life span and allowing collision with the fires with a check of type
/// </summary>

public class Projectile : MonoBehaviour
{
    public Fire.ExtinguisherType extinguisherType = Fire.ExtinguisherType.None;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Fire fire = collision.collider.GetComponent<Fire>();
        if (fire != null)
        {
            bool correct = fire.TryExtinguish(extinguisherType);
            if (correct)
            {
                GameManager.Instance.AddScore(1);
            }
            else if (fire.currentType != Fire.FireType.None)
            {
                GameManager.Instance.WrongChoice(fire);
            }
        }

        Destroy(gameObject);
    }
}
