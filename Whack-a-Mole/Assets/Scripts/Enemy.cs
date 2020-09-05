using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Является ли враг антикротом:")]public bool antiMole;
    private Animator animator;
    public AnimationClip[] clips; // 0 - Idle, 1 - mope appearence
    [HideInInspector] public bool alreadyClicked = false;
    [Header("Время жизни перед исчезновением")]public float timeBeforeDelay = 3f;
    [HideInInspector] public int indexOfPosition = 0;
    private int scoreCheckEnemy; // уровень очков после которого уменьшается время нахождения врага
    private void Start()
    {
        animator = GetComponent<Animator>();
        scoreCheckEnemy = 50; // задаем уровень очков
    }
    private void Update()
    {
        TimeOfAppearenceEnemy(UIManager.instance.pointsScrore, ref scoreCheckEnemy);
    }
    private void OnEnable()
    {
        StartCoroutine(SelfDisable());
    }
    private void OnDisable()
    {
        StopCoroutine(SelfDisable());
        indexOfPosition = 0;
    }
    /// <summary>
    /// Отключения себя и добавдение новго врага
    /// </summary>
    private void DisableMyself()
    {
        if(SpawnController.instance.permisionForSpawn)
            SpawnController.instance.AddEnemy(this.transform.position);
        gameObject.SetActive(false);
        alreadyClicked = false;
    }
    /// <summary>
    /// Логика поведения героя после нажатия на него
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayBeforeDisableMyself()
    {
        alreadyClicked = true;
        if(!antiMole)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.tapOnEnemy);
            UIManager.instance.pointsScrore++;
        }
        if (antiMole)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.tapOnAntiEnemy);
            UIManager.instance.lives--;
        }
        animator.SetTrigger("Tap");
        yield return new WaitForSeconds(clips[1].length);
        DisableMyself();
    }
    /// <summary>
    /// Отключает врага после определнного времени
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(timeBeforeDelay);
        if (!antiMole)
        {
            animator.SetTrigger("Tap");
            yield return new WaitForSeconds(clips[1].length);
            UIManager.instance.lives--;
            SoundManager.instance.PlaySound(SoundManager.instance.loseLive);
        }
        DisableMyself();
    }
    /// <summary>
    /// Уменьшает нахождение врага в зависимости от набранных очков
    /// </summary>
    /// <param name="score">Набранные очки</param>
    /// <param name="scoreCheck">Уровень набранных очков</param>
    private void TimeOfAppearenceEnemy(int score, ref int scoreCheck)
    {
        if (timeBeforeDelay > 0.5f)
        {
            if (score > scoreCheck)
            {
                scoreCheck += 50;
                timeBeforeDelay -= 0.5f;
            }
        }
    }
}
