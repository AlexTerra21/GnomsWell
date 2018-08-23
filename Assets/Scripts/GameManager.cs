using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляет состоянием игры.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Местоположение, где должен появиться гномик.
    /// </summary>
    public GameObject startingPoint;

    /// <summary>
    /// Объект веревки, опускающей и поднимающей гномика.
    /// </summary>
    public Rope rope;

    /// <summary>
    /// Сценарий, управляющий камерой, которая должна следовать за гномиком
    /// </summary>
    public CameraFollow cameraFollow;

    /// <summary>
    /// 'текущий' гномик (в противоположность всем погибшим)
    /// </summary>
    Gnome currentGnome;

    /// <summary>
    /// Объект-шаблон для создания нового гномика
    /// </summary>
    public GameObject gnomePrefab;

    /// <summary>
    /// Компонент пользовательского интерфейса с кнопками 'перезапустить и 'продолжить'
    /// </summary>
    public RectTransform mainMenu;

    /// <summary>
    /// Компонент пользовательского интерфейса с кнопками 'вверх', 'вниз' и 'меню'
    /// </summary>
    public RectTransform gameplayMenu;

    /// <summary>
    /// Компонент пользовательского интерфейса с экраном 'вы выиграли!'
    /// </summary>
    public RectTransform gameOverMenu;

    /// <summary>
    /// Значение true в этом свойстве требует игнорировать любые повреждения  (но показывать визуальные эффекты).
    /// Объявление 'get; set;' превращает поле в свойство, что необходимо для отображения в списке методов в инспекторе для Unity Events
    /// </summary>
    public bool gnomeInvincible { get; set; }

    /// <summary>
    /// Задержка перед созданием нового гномика после гибели
    /// </summary>
    public float delayAfterDeath = 1.0f;

    /// <summary>
    /// Звук, проигрываемый в случае гибели гномика
    /// </summary>
    public AudioClip gnomeDiedSound;

    /// <summary>
    /// Звук, проигрываемый в случае победы в игре
    /// </summary>
    public AudioClip gameOverSound;


    void Start () {
        // В момент запуска игры вызвать Reset, чтобы подготовить гномика.
        Reset();
	}

    /// <summary>
    /// Сбрасывает игру в исходное состояние.
    /// </summary>
    public void Reset ()
    {
        // Выключает меню, включает интерфейс игры
        if (gameOverMenu) gameOverMenu.gameObject.SetActive(false);
        if (mainMenu)     mainMenu.gameObject.SetActive(false);
        if (gameplayMenu) gameplayMenu.gameObject.SetActive(true);

        // Найти все компоненты Resettable и сбросить их в исходное состояние
        var resetObjects = FindObjectsOfType<Resettable>();
        foreach (Resettable r in resetObjects)
        {
            r.Reset();
        }

        // Создать нового гномика
        CreateNewGnome();

        // Прервать паузу в игре
        Time.timeScale = 1.0f;
    }

    private void CreateNewGnome()
    {
        // Удалить текущего гномика, если имеется
        RemoveGnome();

        // Создать новый объект Gnome и назначить его текущим
        GameObject newGnome = (GameObject)Instantiate(gnomePrefab, startingPoint.transform.position, Quaternion.identity);
        currentGnome = newGnome.GetComponent<Gnome>();

        // Показать веревку
        rope.gameObject.SetActive(true);

        // Привязать конец веревки к заданному твердому телу в объекте Gnome (например, к его ноге)
        rope.connectedObject = currentGnome.ropeBoby;

        // Установить длину веревки в начальное значение
        rope.ResetLength();

        // Сообщить объекту cameraFollow, что он должен начать следить за новым объектом Gnome
        cameraFollow.target = currentGnome.cameraFollowTarget;

    }

    private void RemoveGnome()
    {
        // Ничего не делать, если гномик неуязвим
        if (gnomeInvincible)
        {
            return;
        }

        // Скрыть веревку
        rope.gameObject.SetActive(false);

        // Запретить камере следовать за гномиком
        cameraFollow.target = null;

        // Если текущий гномик существует, исключить его из игры
        if (currentGnome != null)
        {
            // Этот гномик больше не удерживает сокровище
            currentGnome.holdingTreasure = false;

            // Пометить объект как исключенный из игры (чтобы коллайдеры перестали сообщать о столкновениях с ним)
            currentGnome.gameObject.tag = "Untagged";

            // Найти все объекты с тегом "Player" и удалить этот тег
            foreach (Transform child in currentGnome.transform)
            {
                child.gameObject.tag = "Untagged";
            }

            // Установить признак отсутствия текущего гномика
            currentGnome = null;
        }







    }
}
