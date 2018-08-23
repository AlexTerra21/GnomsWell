using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Удаляет объект с заданной задержкой.
/// </summary>
public class RemoveAfterDelay : MonoBehaviour {

    /// <summary>
    /// Задержка в секундах перед удалением.
    /// </summary>
    public float delay = 1.0f;

	void Start () {
        // Запустить сопрограмму 'Remove'.
        StartCoroutine("Remove");	
	}

    IEnumerator Remove()
    {
        // Ждать 'delay' секунд и затем уничтожить объект
        // gameObject, присоединенный к объекту this.
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);

        // Нельзя использовать вызов Destroy(this) - он уничтожит сам
        // объект сценария RemoveAfterDelay.
    }
}
