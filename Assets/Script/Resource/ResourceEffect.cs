using System.Collections;
using UnityEngine;

public class ResourceEffect : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("타격 흔들림")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakePower = 0.08f;

    [Header("타격 플래시")]
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    [Header("파괴 효과")]
    [SerializeField] private float destroyDuration = 0.18f;

    [Header("사운드 및 파티클")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ParticleSystem destroyParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroySound;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;

    private Coroutine hitCoroutine;
    private Coroutine destroyCoroutine;

    private bool isDestroying;

    // Resource.cs에서 파괴 연출 시간을 확인할 때 사용
    public float DestroyDuration => destroyDuration;

    private void Awake()
    {
        if (visual == null)
        {
            visual = transform;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer =
                visual.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                spriteRenderer =
                    GetComponentInChildren<SpriteRenderer>();
            }
        }

        originalPosition = visual.localPosition;
        originalScale = visual.localScale;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        // 테스트용
        // H키를 누르면 타격 효과
        // J키를 누르면 파괴 효과
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayHitEffect();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayDestroyEffect();
        }
    }

    public void PlayHitEffect()
    {
        if (isDestroying)
        {
            return;
        }

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        hitCoroutine = StartCoroutine(
            HitEffectCoroutine()
        );

        if (hitParticle != null)
        {
            hitParticle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            hitParticle.Play();
        }

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    public void PlayDestroyEffect()
    {
        if (isDestroying)
        {
            return;
        }

        isDestroying = true;

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        destroyCoroutine = StartCoroutine(
            DestroyEffectCoroutine()
        );
    }

    private IEnumerator HitEffectCoroutine()
    {
        float elapsedTime = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
        }

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(
                -shakePower,
                shakePower
            );

            float y = Random.Range(
                -shakePower * 0.35f,
                shakePower * 0.35f
            );

            visual.localPosition =
                originalPosition +
                new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visual.localPosition = originalPosition;

        yield return new WaitForSeconds(
            flashDuration
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        hitCoroutine = null;
    }

    private IEnumerator DestroyEffectCoroutine()
    {
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }

        if (destroyParticle != null)
        {
            destroyParticle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            destroyParticle.Play();
        }

        float elapsedTime = 0f;

        while (elapsedTime < destroyDuration)
        {
            float progress =
                elapsedTime / destroyDuration;

            // 살짝 커졌다가 빠르게 작아지는 효과
            float scaleMultiplier;

            if (progress < 0.2f)
            {
                scaleMultiplier = Mathf.Lerp(
                    1f,
                    1.1f,
                    progress / 0.2f
                );
            }
            else
            {
                scaleMultiplier = Mathf.Lerp(
                    1.1f,
                    0f,
                    (progress - 0.2f) / 0.8f
                );
            }

            visual.localScale =
                originalScale * scaleMultiplier;

            visual.localPosition =
                originalPosition +
                new Vector3(
                    Random.Range(-0.04f, 0.04f),
                    Random.Range(-0.02f, 0.05f),
                    0f
                );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visual.localScale = Vector3.zero;
        visual.localPosition = originalPosition;

        destroyCoroutine = null;

        // 여기에서 Destroy(gameObject)를 하면 안 됨
    }

    public void ResetEffect()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }

        isDestroying = false;

        visual.localPosition = originalPosition;
        visual.localScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (hitParticle != null)
        {
            hitParticle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        if (destroyParticle != null)
        {
            destroyParticle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }
}