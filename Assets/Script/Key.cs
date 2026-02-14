using UnityEngine;

public class Key : MonoBehaviour
{
    public Door door;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
            activated = true;
            door.OpenDoor();
        }
    }
}
