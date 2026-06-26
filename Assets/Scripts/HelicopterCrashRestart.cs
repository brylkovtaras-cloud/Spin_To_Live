using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HelicopterCrashRestart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator explosionAnimator;

    [Header("Crash Detection")]
    [SerializeField] private float badTiltDotThreshold = 0.35f;
    [SerializeField] private float stoppedSpeedThreshold = 0.15f;
    [SerializeField] private float crashDelay = 3f;

    [Header("Restart")]
    [SerializeField] private float restartAfterExplosion = 1f;

    private float crashTimer;
    private bool isRestarting;

    private void Awake()
    {
        if (body == null)
            body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            RestartLevel();

        CheckCrashState();
    }

    private void CheckCrashState()
    {
        if (isRestarting)
            return;

        float uprightAmount = Vector2.Dot(transform.up, Vector2.up);
        bool badlyTilted = uprightAmount < badTiltDotThreshold;
        bool barelyMoving = body.linearVelocity.magnitude < stoppedSpeedThreshold;

        if (badlyTilted && barelyMoving)
        {
            crashTimer += Time.deltaTime;

            if (crashTimer >= crashDelay)
                StartCoroutine(ExplodeAndRestart());
        }
        else
        {
            crashTimer = 0f;
        }
    }

    private IEnumerator ExplodeAndRestart()
    {
        isRestarting = true;

        if (explosionAnimator != null)
            explosionAnimator.SetTrigger("Explode");

        yield return new WaitForSeconds(restartAfterExplosion);

        RestartLevel();
    }

    private void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}