using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour
{
    // Спрайт, используемый в вызове ApplyDamageSprite
    // с повреждением типа 'порез'
    public Sprite detachedSprite;

    // Спрайт, используемый в вызове ApplyDamageSprite
    // с повреждением типа 'ожог'
    public Sprite burnedSprite;

    // Представляет позицию и поворот для отображения фонтана
    // крови, бьющего из основного тела
    public Transform bloodFountainOrigin;

    // Если имеет значение true, после падения из этого объекта должны
    // быть удалены все коллизии, сочленения и твердое тело
    bool detachd = false;


    /// <summary>
    /// Отделяет объект this от родителя и устанавливает флаг,
    /// требующий удаления физических свойств
    /// </summary>
    public void Detach () {
        detachd = true;

        this.tag = "Untagged";

        this.transform.SetParent(null, true);
	}

    // В каждом кадре, если часть тела отделена от основного тела,
    // удаляет физические характеристики после достижения дна колодца.
    // Это означает, что отделенная часть тела не будет мешать
    // гномику взять сокровище.
    void Update ()
    {
        // Если часть тела не отделена, ничего не делать
        if (detachd == false)
        {
            return;
        }

        // Твердое тело прекратило падение?
        Rigidbody2D rigibody = GetComponent<Rigidbody2D>();

        if (rigibody.IsSleeping())
        {
            // Если да, удалить все сочленения...
            foreach (Joint2D joint in GetComponentsInChildren<Joint2D>())
            {
                Destroy(joint);
            }

            // ...твердые тела...
            foreach (Rigidbody2D body in GetComponentsInChildren<Rigidbody2D>())
            {
                Destroy(body);
            }

            // ...и коллайдеры.
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                Destroy(collider);
            }

            // В конце удалить компонент с этим сценарием.
            Destroy(this);
        }
    }

    
    /// <summary>
    /// Заменяет спрайт этой части тела, исходя из
    /// вида полученного повреждения
    /// </summary>
    /// <param name="damageType">Тип повреждения</param>
    public void ApplyDamageSprite(Gnome.DamageType damageType)
    {
        Sprite spriteToUse = null;

        switch (damageType)
        {
            case Gnome.DamageType.Slicing:
                spriteToUse = detachedSprite;
                break;
            case Gnome.DamageType.Burning:
                spriteToUse = burnedSprite;
                break;
            default:
                break;
        }

        if (spriteToUse != null)
        {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }

    }

}
