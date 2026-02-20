using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        [Header("Score")]
        public int coinsCounter = 0;
        public int highScore;
        public TMP_Text finalDeathText;
        public TMP_Text deathHighScoreText;
        public TMP_Text finalWinText;
        public TMP_Text winHighScoreText;

        [Header("References")]
        public GameObject playerGameObject;
        public GameObject mainUI;
        public GameObject deathUI;
        public GameObject winUI;
        public GameObject deathPlayerPrefab;

        [Header("UI")]
        public TMP_Text scoreText;

        [Header("Audio")] // 🔥 เพิ่มตรงนี้
        public AudioSource bgmSource;
        public AudioSource sfxSource;
        public AudioClip deathSound;
        public AudioClip winSound;

        private PlayerController player;
        private Vector3 currentCheckpointPosition;

        void Start()
        {
            deathUI.SetActive(false);
            winUI.SetActive(false);

            player = playerGameObject.GetComponent<PlayerController>();

            // 🔥 เล่น BGM ตอนเริ่มเกม
            if (bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.Play();
            }

            if (PlayerPrefs.HasKey("CheckpointX"))
            {
                float x = PlayerPrefs.GetFloat("CheckpointX");
                float y = PlayerPrefs.GetFloat("CheckpointY");
                float z = PlayerPrefs.GetFloat("CheckpointZ");

                Vector3 checkpointPos = new Vector3(x, y, z);
                playerGameObject.transform.position = checkpointPos;
            }
            CoinManager.Instance.LoadCoins();
        }

        void Update()
        {
            coinsCounter = CoinManager.Instance.coinCount;
            scoreText.text = coinsCounter.ToString();

            if (player != null && player.deathState)
            {
                HandlePlayerDeath();
            }
            if (player != null && player.winState)
            {
                HandlePlayerWin();
            }
        }

        void ClearCheckpointData()
        {
            PlayerPrefs.DeleteKey("SavedLevel");
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
            PlayerPrefs.DeleteKey("CheckpointZ");
        }

        void HandlePlayerDeath()
        {
            EndGame();

            // 🔥 ปิด BGM
            if (bgmSource != null)
                bgmSource.Stop();

            // 🔥 เล่นเสียงตาย
            if (deathSound != null)
                sfxSource.PlayOneShot(deathSound);

            playerGameObject.SetActive(false);

            GameObject deathPlayer = Instantiate(
                deathPlayerPrefab,
                playerGameObject.transform.position,
                playerGameObject.transform.rotation
            );

            deathPlayer.transform.localScale = playerGameObject.transform.localScale;

            player.deathState = false;

            finalDeathText.text = coinsCounter.ToString();
            deathHighScoreText.text = "High Score: " + highScore.ToString();

            mainUI.SetActive(false);
            deathUI.SetActive(true);
        }

        public void Respawn()
        {
            deathUI.SetActive(false);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            playerGameObject.SetActive(true);
        }

        void HandlePlayerWin()
        {
            EndGame();

            // 🔥 ปิด BGM
            if (bgmSource != null)
                bgmSource.Stop();

            // 🔥 เล่นเสียงชนะ
            if (winSound != null)
                sfxSource.PlayOneShot(winSound);

            playerGameObject.SetActive(false);

            finalWinText.text = coinsCounter.ToString();
            winHighScoreText.text = "High Score: " + highScore.ToString();

            mainUI.SetActive(false);
            winUI.SetActive(true);
        }

        public void ReloadLevel()
        {
            ResetAll();
            ClearCheckpointData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void EndGame()
        {
            EndHighScore();
        }

        public void EndHighScore()
        {
            if (coinsCounter > highScore)
            {
                highScore = coinsCounter;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
        }

        public void ResetAll()
        {
            for (int i = 1; i <= 50; i++)
            {
                PlayerPrefs.DeleteKey("Coin_Coin" + i);
            }

            PlayerPrefs.DeleteKey("SavedLevel");
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
            PlayerPrefs.DeleteKey("CheckpointZ");
            PlayerPrefs.DeleteKey("SavedCoins");

            PlayerPrefs.Save();
            Debug.Log("All Coins Reset");
        }
    }
}