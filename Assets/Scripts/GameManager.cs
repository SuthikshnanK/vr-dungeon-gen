using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;

    Canvas UICanvas;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void EndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            GameObject.FindObjectOfType<AudioListener>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            CanvasGroup gameoverCanvas = GameObject.FindGameObjectWithTag("GameOver").GetComponent<CanvasGroup>();

            StartCoroutine(DoFade(gameoverCanvas, 0f, 1f, 2f));
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator DoFade(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float counter = 0f;

        while(counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
    }
}
