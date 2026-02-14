using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class Checkpoint : MonoBehaviour
    {
        private bool activated = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!activated && other.CompareTag("Player"))
            {
                activated = true;

                Vector3 pos = transform.position;

                // à«¿ª×èÍ©Ò¡
                PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);

                // à«¿µÓáË¹è§
                PlayerPrefs.SetFloat("CheckpointX", pos.x);
                PlayerPrefs.SetFloat("CheckpointY", pos.y);
                PlayerPrefs.SetFloat("CheckpointZ", pos.z);

                PlayerPrefs.Save();

                Debug.Log("Checkpoint Saved!");
            }
        }
    }
}
