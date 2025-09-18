using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string extinguisherType; // "CO2" or "Water"
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
                GameManager.Instance.AddScore(1);
            else
                GameManager.Instance.WrongChoice();
        }
        Destroy(gameObject);
    }
}
