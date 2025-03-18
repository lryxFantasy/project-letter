using UnityEngine;

public class CharacterSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "character"; // 设置为 character层
    }

    void Update()
    {
        // 根据 Y 位置调整 Order in Layer
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}