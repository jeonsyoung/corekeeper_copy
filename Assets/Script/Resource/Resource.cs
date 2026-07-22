using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header("자원 체력")]
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    [Header("드랍 아이템")]
    [SerializeField] private List<DropItem> drops = new();

    [Header("리젠")]
    [SerializeField] private float respawnTime = 8f;

    private ResourceEffect resourceEffect;
    private SpriteRenderer spriteRenderer;
    private Collider2D resourceCollider;

    private bool isBroken = false;

    private void Awake()
    {
        currentHp = maxHp;

        // 같은 오브젝트에 붙어있는 ResourceEffect 스크립트를 가져옴
        resourceEffect = GetComponent<ResourceEffect>();

        // SpriteRenderer가 자식 Visual에 있어도 찾을 수 있게 함
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        resourceCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name}: SpriteRenderer를 찾지 못했습니다.");
        }

        if (resourceCollider == null)
        {
            Debug.LogError($"{gameObject.name}: Collider2D를 찾지 못했습니다.");
        }
    }

    // 플레이어가 타격할 때 호출되는 함수
    public void TakeDamage(int damage)
    {
        // 이미 파괴 처리 중이면 더 이상 실행하지 않음
        if (isBroken || damage <= 0)
        {
            return;
        }

        currentHp -= damage;
        Debug.Log($"{gameObject.name} 타격받음! (남은 HP: {currentHp}/{maxHp})");

        // 1. 피격 이펙트 재생 (흔들림 + 깜빡임)
        if (resourceEffect != null)
        {
            resourceEffect.PlayHitEffect();
        }

        // 2. HP가 0 이하가 되면 파괴 이펙트 재생 후 삭제
        if (currentHp <= 0)
        {
            DestroyResource();
        }
        else
        {
            resourceEffect?.PlayHitEffect();
        }
    }

    private void DestroyResource()
    {
        if (isBroken)
        {
            return;
        }

        // 가장 먼저 잠금
        isBroken = true;

        // 추가 공격 방지
        if (resourceCollider != null)
        {
            resourceCollider.enabled = false;
        }

        // 드랍은 여기에서 딱 한 번만
        DropItems();

        // 파괴 효과
        resourceEffect?.PlayDestroyEffect();

        // 리젠 시작
        StartCoroutine(RespawnCoroutine());
    }

    private void DropItems()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("Inventory.Instance를 찾을 수 없습니다.");
            return;
        }
        Debug.Log("Drop!");
        foreach (DropItem drop in drops)
        {
            if (drop == null || drop.item == null)
            {
                continue;
            }

            int randomValue = Random.Range(0, 100);

            if (randomValue >= drop.chance)
            {
                continue;
            }

            int minimum = Mathf.Max(1, drop.minAmount);
            int maximum = Mathf.Max(minimum, drop.maxAmount);

            int amount = Random.Range(minimum, maximum + 1);

            Inventory.Instance.AddItem(drop.item, amount);

            Debug.Log(
                $"{drop.item.itemName} {amount}개 획득"
            );
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        // 파괴 애니메이션이 끝날 때까지 기다림
        if (resourceEffect != null)
        {
            yield return new WaitForSeconds(
                resourceEffect.DestroyDuration
            );
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 파괴된 뒤 리젠까지 기다리는 시간
        yield return new WaitForSeconds(respawnTime);

        currentHp = maxHp;

        if (resourceEffect != null)
        {
            resourceEffect.ResetEffect();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (resourceCollider != null)
        {
            resourceCollider.enabled = true;
        }

        isBroken = false;

        Debug.Log($"{gameObject.name} 리젠 완료");
    }
}