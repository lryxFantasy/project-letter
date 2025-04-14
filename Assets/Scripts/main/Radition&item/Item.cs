using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Battery, Shield }

    [SerializeField] public ItemType itemType = ItemType.Battery; // 道具类型
    [SerializeField] private int healthRestore = 20; // 电池恢复血量
    [SerializeField] private float shieldDuration = 3f; // 护盾持续时间
    [SerializeField] private GameObject pickupEffect; // 粒子效果（电池和护盾共用）

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RubyController player = other.GetComponent<RubyController>();
            if (player != null && !player.GetComponent<PlayerController>().IsInDialogue())
            {
                if (itemType == ItemType.Battery)
                {
                    player.ChangeHealth(healthRestore);
                }
                else if (itemType == ItemType.Shield)
                {
                    player.ActivateShield(shieldDuration);
                }

                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayPickupSound();
                }

                if (pickupEffect != null)
                {
                    GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    Destroy(effect, 0.5f); // 0.5秒后销毁粒子
                }

                ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
                if (spawner != null)
                {
                    // 传递道具类型和位置
                    spawner.OnItemPickedUp(itemType, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}