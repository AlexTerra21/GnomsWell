using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Содержит поле UnityEvent, которое используется для
/// установки объекта this в исходное состояние.
/// </summary>
public class Resettable : MonoBehaviour
{
    /// <summary>
    /// В редакторе подключите это событие к методам, которые должны вызываться в момент сброса игры.
    /// </summary>
    public UnityEvent onReset;

    /// <summary>
    /// Вызывается диспетчером игры GameManager в момент сброса игры.
    /// </summary>
	public void Reset ()
    {
        // Породить событие, которое вызовет все
        // подключенные методы.
        onReset.Invoke();
	}
}
