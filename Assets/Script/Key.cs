using UnityEngine;

public class Key : MonoBehaviour
{
    public Door door;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;

            // 🔑 พลิก Key
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;

            // 🔊 เล่นเสียงหลังจากพลิก
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySoundAtPosition(
                    SoundManager.Instance.lever,
                    transform.position
                );
            }

            // 🚪 เปิดประตู
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }
}