using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;

    public AudioClip footstep;
    public AudioClip jump;
    public AudioClip wallJump;
    public AudioClip land;
    public AudioClip death;
    public AudioClip coin;
    public AudioClip lever;
    public AudioClip win;

    void Awake()
    {
        Instance = this;
    }

    // 🔊 เล่นเสียงปกติ
    public void PlaySound(AudioClip clip, float pitchMin = 1f, float pitchMax = 1f)
    {
        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip);
    }

    // 💥 เล่นเสียงแม้ Object ถูกลบ
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float pitchMin = 1f, float pitchMax = 1f)
    {
        GameObject soundObj = new GameObject("SFX_" + clip.name);
        soundObj.transform.position = position;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
        audioSource.Play();

        Destroy(soundObj, clip.length);
    }
}