using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 3f, 0); // ระยะเลื่อนตอนเปิด
    public float openSpeed = 2f;
    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

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
        }
    }

    public void OpenDoor()
    {
        isOpening = true;
    }
}
