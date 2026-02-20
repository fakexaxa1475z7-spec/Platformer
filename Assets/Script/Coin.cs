using UnityEngine;

public class Coin : MonoBehaviour
{
    public string coinID;

    private void Start()
    {
        // ถ้าเคยเก็บแล้ว ให้ลบทิ้งทันที
        if (PlayerPrefs.GetInt("Coin_" + coinID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }
}