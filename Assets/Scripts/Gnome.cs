using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    // Объект, за которым должна следовать камера.
    public Transform cameraFollowTarget;

    public Rigidbody2D ropeBoby;

    public Sprite armHoldingEmpty;
    public Sprite armHoldingTreasure;

    public SpriteRenderer holdingArm;

    public GameObject deathPrefab;
    public GameObject flameDeathPrefab;
    public GameObject ghostPrefab;

    public float delayBeforeRemoving = 3.0f;
    public float delayBeforeReleasingGhost = 0.25f;

    public GameObject bloodFontainPrefab;

    bool _dead = false;

    bool _holdingTreasure = false;

    public bool holdingTreasure
    {
        get
        {
            return _holdingTreasure;
        }
        set
        {
            if (_dead == true)
            {
                return;
            }

            _holdingTreasure = value;

            if (holdingArm != null)
            {
                if (_holdingTreasure)
                {
                    holdingArm.sprite = armHoldingTreasure;
                }
                else
                {
                    holdingArm.sprite = armHoldingEmpty;
                }
            }
        }
    }

    public enum DamageType
    {
        Slicing,
        Burning
    }

    public void ShowDamageEffect(DamageType type)
    {
        switch (type)
        {
            case DamageType.Slicing:
                if (deathPrefab != null)
                {
                    Instantiate(deathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;
            case DamageType.Burning:
                if (flameDeathPrefab != null)
                {
                    Instantiate(flameDeathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;
            default:
                break;
        }
    }

    public void DestroyGnome(DamageType type)
    {
        holdingTreasure = false;

        _dead = true;

        // найти все дочерние объекты и произвольно
        // отсоединить их сочленения
        foreach (BodyPart part in GetComponentsInChildren<BodyPart>())
        {
            switch (type)
            {
                case DamageType.Burning:
                    // один шанс из трех получить ожог
                    bool shouldBurn = Random.Range(0, 2) == 0;
                    if (shouldBurn)
                    {
                        part.ApplyDamageSprite(type);
                    }
                    break;
                case DamageType.Slicing:
                    // Отсечение части тела всегда влечет смену спрайта
                    part.ApplyDamageSprite(type);
                    break;
                default:
                    break;
            }

            // один шанс из трех отделения от тела
            bool shouldDetach = Random.Range(0, 2) == 0;
            if (shouldDetach)
            {
                // Обеспечить удаление твердого тела и коллайдера
                // из этого объекта после достижения дна
                part.Detach();

                // Если часть тела отделена и повреждение имеет
                // тип Slicing, добавить фонтан крови

                if (type == DamageType.Slicing)
                {
                    if (part.bloodFountainOrigin != null && bloodFontainPrefab != null)
                    {
                        // Присоединить фонтан крови
                        // к отделившейся части тела
                        GameObject fountain = Instantiate(bloodFontainPrefab, part.bloodFountainOrigin.position, part.bloodFountainOrigin.rotation) as GameObject;
                        fountain.transform.SetParent(this.cameraFollowTarget, false);
                    }
                }
                // Отделить объект this
                var allJoins = part.GetComponentsInChildren<Joint2D>();
                foreach (Joint2D jont in allJoins)
                {
                    Destroy(jont);
                }
            }
        }
        // Добавить компонент RemoveAfterDelay в объект this
        var remove = gameObject.AddComponent<RemoveAfterDelay>();
        remove.delay = delayBeforeRemoving;
        StartCoroutine(ReleaseGhost());
    }

    IEnumerator ReleaseGhost()
    {
        // Шаблон духа не определен? Выйти.
        if (ghostPrefab == null)
        {
            yield break;
        }

        // Ждать delayBeforeReleasingGhost секунд
        yield return new WaitForSeconds(delayBeforeReleasingGhost);

        // Добавить дух
        Instantiate(ghostPrefab, transform.position,Quaternion.identity);
    }

}
