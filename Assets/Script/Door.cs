using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openSpeed = 2f;

    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool hasPlayedSound = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;
    }

    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * openSpeed);

            // หยุดตอนเปิดสุด
            if (Vector3.Distance(transform.position, openPosition) < 0.01f)
            {
                transform.position = openPosition;
                isOpening = false;
            }
        }
    }

    public void OpenDoor()
    {
        if (isOpening) return;

        isOpening = true;

        // 🔊 เล่นเสียง
        if (!hasPlayedSound && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.lever);
            hasPlayedSound = true;
        }
    }
}