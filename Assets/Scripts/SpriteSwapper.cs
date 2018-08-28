using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Меняет один спрайт на другой. Например, при переключении сокровища из состояния 'сокровище есть' в состояние 'сокровища нет'.
/// </summary>
public class SpriteSwapper : MonoBehaviour
{
    /// <summary>
    /// Спрайт, который требуется отобразить.
    /// </summary>
    public Sprite spriteToUse;

    /// <summary>
    /// Визуализатор спрайта, который должен использоваться для отображения нового спрайта.
    /// </summary>
    public SpriteRenderer spriteRenderer;

    /// <summary>
    /// Исходный спрайт. Используется в вызове ResetSprite.
    /// </summary>
    private Sprite originalSprite;

    /// <summary>
    /// Меняет спрайт.
    /// </summary>
    public void SwapSprite()
    {
        // Если требуемый спрайт отличается от текущего...
        if (spriteToUse != spriteRenderer.sprite)
        {
            // Сохранить предыдущий в originalSprite
            originalSprite = spriteRenderer.sprite;

            // Передать новый спрайт визуализатору.
            spriteRenderer.sprite = spriteToUse;
        }	
	}

    /// <summary>
    /// Возвращает прежний спрайт.
    /// </summary>
    public void ResetSprite()
    {
        // Если прежний спрайт был сохранен...
        if (originalSprite != null)
        {
            // ...передать его визуализатору.
            spriteRenderer.sprite = originalSprite;
        }
	}
}
