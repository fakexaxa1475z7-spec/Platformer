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
        public TMP_Text timerText;
        public TMP_Text finalDeathText;
        public TMP_Text deathHighScoreText;
        public TMP_Text DeathimerText;
        public TMP_Text deathHighTimerText;
        public TMP_Text finalWinText;
        public TMP_Text winHighScoreText;
        public TMP_Text winHighTimerText;
        public TMP_Text winTimerText;

        [Header("Timer")]
        private float currentTime = 0f;
        private bool isTiming = true;

        [Header("References")]
        public GameObject playerGameObject;
        public GameObject mainUI;
        public GameObject deathUI;
        public GameObject winUI;
        public GameObject deathPlayerPrefab;

        [Header("UI")]
        public TMP_Text scoreText;

        private PlayerController player;
        private Vector3 currentCheckpointPosition;

        void Start()
        {
            currentTime = 0f;
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            float bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);

            deathUI.SetActive(false);
            winUI.SetActive(false);

            player = playerGameObject.GetComponent<PlayerController>();

            if (PlayerPrefs.HasKey("CheckpointX"))
            {
                float x = PlayerPrefs.GetFloat("CheckpointX");
                float y = PlayerPrefs.GetFloat("CheckpointY");
                float z = PlayerPrefs.GetFloat("CheckpointZ");

                Vector3 checkpointPos = new Vector3(x, y, z);
                playerGameObject.transform.position = checkpointPos;
            }

        }

        void Update()
        {
            scoreText.text = coinsCounter.ToString();

            if (player != null && player.deathState)
            {
                HandlePlayerDeath();
            }
            if (player != null && player.winState)
            {
                HandlePlayerWin();
            }
            if (isTiming)
            {
                currentTime += Time.deltaTime;
                UpdateTimerUI();
            }
        }

        void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            int milliseconds = Mathf.FloorToInt((currentTime * 100) % 100);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}",
                minutes, seconds, milliseconds);
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
            DeathimerText.text = "Time: " + timerText.text;
            deathHighTimerText.text = "Best Time: " + string.Format("{0:00}:{1:00}:{2:00}",
                Mathf.FloorToInt(PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) / 60),
                Mathf.FloorToInt(PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) % 60),
                Mathf.FloorToInt((PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) * 100) % 100));

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
            playerGameObject.SetActive(false);

            finalWinText.text = coinsCounter.ToString();
            winHighScoreText.text = "High Score: " + highScore.ToString();
            winTimerText.text = "Time: " + timerText.text;
            winHighTimerText.text = "Best Time: " + string.Format("{0:00}:{1:00}:{2:00}",
                Mathf.FloorToInt(PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) / 60),
                Mathf.FloorToInt(PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) % 60),
                Mathf.FloorToInt((PlayerPrefs.GetFloat("BestTime", Mathf.Infinity) * 100) % 100));

            mainUI.SetActive(false);
            winUI.SetActive(true);
        }

        public void ReloadLevel()
        {
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
            EndBestTime();
            isTiming = false;
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
        public void EndBestTime()
        {
            if (currentTime < PlayerPrefs.GetFloat("BestTime", Mathf.Infinity))
            {
                PlayerPrefs.SetFloat("BestTime", currentTime);
                PlayerPrefs.Save();
            }
        }
    }
}
