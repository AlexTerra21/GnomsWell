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

    /// <summary>
    /// Убивает гномика.
    /// </summary>
    /// <param name="damageType"></param>
    void KillGnome(Gnome.DamageType damageType)
    {
        // Если задан источник звука, проиграть звук "гибель гномика"
        var audio = GetComponent<AudioSource>();
        if (audio)
        {
            audio.PlayOneShot(this.gnomeDiedSound);
        }

        // Показать эффект действия ловушки
        currentGnome.ShowDamageEffect(damageType);

        // Если гномик уязвим, сбросить игру и исключить гномика из игры.
        if (gnomeInvincible == false)
        {
            // Сообщить гномику, что он погиб
            currentGnome.DestroyGnome(damageType);

            // Удалить гномика
            RemoveGnome();

            // Сбросить игру
            StartCoroutine(ResetAfterDelay());
        }
    }

    /// <summary>
    /// Вызывается в момент гибели гномика.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetAfterDelay()
    {
        // Ждать delayAfterDeath секунд, затем вызвать Reset
        yield return new WaitForSeconds(delayAfterDeath);
        Reset();
    }

    /// <summary>
    /// Вызывается, когда гномик касается ловушки с ножами
    /// </summary>
    public void TrapTouched()
    {
        KillGnome(Gnome.DamageType.Slicing);
    }

    /// <summary>
    /// Вызывается, когда гномик касается огненной ловушки
    /// </summary>
    public void FireTrapTouched()
    {
        KillGnome(Gnome.DamageType.Burning);
    }

    /// <summary>
    /// Вызывается, когда гномик касается сокровища.
    /// </summary>
    public void TreasureCollected()
    {
        currentGnome.holdingTreasure = true;
    }

    /// <summary>
    /// Вызывается, когда гномик касается выхода.
    /// </summary>
    public void ExitReached()
    {
        // Завершить игру, если есть гномик и он держит сокровище!
        if (currentGnome != null && currentGnome.holdingTreasure == true)
        {
            //Если задан источник звука, проиграть звук "игра завершена"
            var audio = GetComponent<AudioSource>();
            if (audio)
            {
                audio.PlayOneShot(this.gameOverSound);
            }

            // Приостановить игру
            Time.timeScale = 0.0f;

            // Выключить меню завершения игры и включить экран "игра завершена"!
            if (gameOverMenu)
            {
                gameOverMenu.gameObject.SetActive(true);
            }

            if (gameplayMenu)
            {
                gameplayMenu.gameObject.SetActive(true);
            }
        }
        RestarmGame();
    }

    /// <summary>
    /// // Вызывается в ответ на касание кнопок Menu и Resume Game.
    /// </summary>
    /// <param name="paused"></param>
    public void SetPaused(bool paused)
    {
        // Если игра на паузе, остановить время и включить меню (и выключить интерфейс игры)
        if (paused)
        {
            Time.timeScale = 0.0f;
            mainMenu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
        }
        else
        {
            // Если игра не на паузе, возобновить ход времени и выключить меню (и включить интерфейс игры)
            Time.timeScale = 1.0f;
            mainMenu.gameObject.SetActive(false);
            gameplayMenu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    ///  Вызывается в ответ на касание кнопки Restart.
    /// </summary>
    public void RestarmGame()
    {
        // Немедленно удалить гномика (минуя этап гибели)
        Destroy(currentGnome.gameObject);
        currentGnome = null;

        // Сбросить игру в исходное состояние, чтобы создать нового гномика.
        Reset();
    }


































}
