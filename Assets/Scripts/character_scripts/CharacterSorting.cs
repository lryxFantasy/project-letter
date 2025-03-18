using UnityEngine;

public class CharacterSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "character"; // ����Ϊ character��
    }

    void Update()
    {
        // ���� Y λ�õ��� Order in Layer
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}