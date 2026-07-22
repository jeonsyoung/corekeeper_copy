using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header("자원 체력")]
    public int maxHp = 3;
    private int currentHp;

    private ResourceEffect resourceEffect;

    private void Awake()
    {
        currentHp = maxHp;
        // 같은 오브젝트에 붙어있는 ResourceEffect 스크립트를 가져옴
        resourceEffect = GetComponent<ResourceEffect>();
    }

    // 플레이어가 타격할 때 호출되는 함수
    public void TakeDamage(int damage)
    {
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
    }

    private void DestroyResource()
    {
        if (resourceEffect != null)
        {
            // 이펙트 스크립트 안에서 파괴 연출 후 Destroy(gameObject)까지 처리해줌
            resourceEffect.PlayDestroyEffect();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}