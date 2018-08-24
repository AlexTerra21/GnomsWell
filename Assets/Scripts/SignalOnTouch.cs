using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Вызывает UnityEvent, когда объект с тегом "Player" касается данного объекта.
/// </summary>
[RequireComponent (typeof(Collider2D))]
public class SignalOnTouch : MonoBehaviour {

    /// <summary>
    /// UnityEvent для выполнения в ответ на касание. Вызываемый метод подключается в редакторе.
    /// </summary>
    public UnityEvent onTouch;

    /// <summary>
    /// Если имеется значение true, при касании проигрывается звук из AudioSource.
    /// </summary>
    public bool playAudioOnTouch = true;

    /// <summary>
    /// Когда обнаруживается вход в область действия триггера, вызывается SendSignal.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        SendSignal(collider.gameObject);
    }

    /// <summary>
    /// Когда обнаруживается касание с данным объектом, вызывается SendSignal.
    /// </summary>
    /// <param name="collider"></param>
    void OnCollisionEnter2D(Collision2D collider)
    {
        SendSignal(collider.gameObject);
    }

    /// <summary>
    /// Проверяет наличие тега "Player" у данного объекта и вызывает UnityEvent, если такой тег имеется.
    /// </summary>
    /// <param name="gameObject"></param>
    private void SendSignal(GameObject objectThatHit)
    {
        if (objectThatHit.CompareTag("Player"))
        {
            // Если требуется воспроизвести звук, попытаться сделать это
            if (playAudioOnTouch)
            {
                var audio = GetComponent<AudioSource>();
                // Если имеется аудиокомпонент и родитель этого компонента активен, воспроизвести звук
                if (audio && audio.gameObject.activeInHierarchy)
                {
                    audio.Play();
                }
            }

            // Вызвать событие
            onTouch.Invoke();
        }
    }





}
