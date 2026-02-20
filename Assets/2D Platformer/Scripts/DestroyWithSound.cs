using UnityEngine;

public class DestroyWithSound : MonoBehaviour
{
    public AudioClip destroySound;
    public bool use3D = false;

    public void DestroyObject()
    {
        if (destroySound != null)
        {
            if (use3D)
            {
                SoundManager.Instance.PlaySoundAtPosition(destroySound, transform.position);
            }
            else
            {
                SoundManager.Instance.PlaySound(destroySound);
            }
        }

        Destroy(gameObject);
    }
}