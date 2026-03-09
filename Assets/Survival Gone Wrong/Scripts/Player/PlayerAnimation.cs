using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public enum AnimState
    {
        Idle,
        Move,
        Run,
        Crouch
    }

    [Header("Sprite Renderer")]
    public SpriteRenderer spriteRenderer;

    [Header("Animations")]
    public List<Sprite> idleSprites;
    public List<Sprite> moveSprites;
    public List<Sprite> runSprites;
    public List<Sprite> crouchSprites;

    [Header("Animation Speed")]
    public float idleFPS = 4f;
    public float moveFPS = 8f;
    public float runFPS = 12f;
    public float crouchFPS = 4f;

    private AnimState currentState;

    private float timer;
    private int frame;

    private List<Sprite> currentSprites;
    private float currentFPS;

    void Update()
    {
        Animate();
    }

    void Animate()
    {
        if (currentSprites == null || currentSprites.Count == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / currentFPS)
        {
            timer = 0f;

            frame++;
            if (frame >= currentSprites.Count)
                frame = 0;

            spriteRenderer.sprite = currentSprites[frame];
        }
    }

    public void SetState(AnimState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        frame = 0;
        timer = 0f;

        switch (newState)
        {
            case AnimState.Idle:
                currentSprites = idleSprites;
                currentFPS = idleFPS;
                break;

            case AnimState.Move:
                currentSprites = moveSprites;
                currentFPS = moveFPS;
                break;

            case AnimState.Run:
                currentSprites = runSprites;
                currentFPS = runFPS;
                break;

            case AnimState.Crouch:
                currentSprites = crouchSprites;
                currentFPS = crouchFPS;
                break;
        }
    }
}