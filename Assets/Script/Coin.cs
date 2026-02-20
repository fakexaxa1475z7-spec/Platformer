using UnityEngine;

public class Coin : MonoBehaviour
{
    public string coinID;

    private void Start()
    {
        // ถ้าเคยเก็บแล้ว -> ลบเลย
        if (PlayerPrefs.GetInt("Coin_" + coinID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        // 🔊 เล่นเสียงก่อน
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundAtPosition(
                SoundManager.Instance.coin,
                transform.position
            );
        }

        // 💾 เซฟว่าเก็บแล้ว
        PlayerPrefs.SetInt("Coin_" + coinID, 1);
        PlayerPrefs.Save();

        // ❌ ลบเหรียญ
        Destroy(gameObject);
    }
}