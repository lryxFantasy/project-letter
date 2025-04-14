using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Battery, Shield }

    [SerializeField] public ItemType itemType = ItemType.Battery; // ��������
    [SerializeField] private int healthRestore = 20; // ��ػָ�Ѫ��
    [SerializeField] private float shieldDuration = 3f; // ���ܳ���ʱ��
    [SerializeField] private GameObject pickupEffect; // ����Ч������غͻ��ܹ��ã�

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
                    Destroy(effect, 0.5f); // 0.5�����������
                }

                ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
                if (spawner != null)
                {
                    // ���ݵ������ͺ�λ��
                    spawner.OnItemPickedUp(itemType, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}