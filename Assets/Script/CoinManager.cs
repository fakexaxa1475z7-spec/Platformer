using UnityEngine;

namespace Platformer
{
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance;

        public int coinCount = 0;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            LoadCoins();
        }

        public void AddCoin(int amount)
        {
            coinCount += amount;
            SaveCoins();
        }

        public void SaveCoins()
        {
            PlayerPrefs.SetInt("SavedCoins", coinCount);
            PlayerPrefs.Save();
        }

        public void LoadCoins()
        {
            coinCount = PlayerPrefs.GetInt("SavedCoins", 0);
        }
    }
}