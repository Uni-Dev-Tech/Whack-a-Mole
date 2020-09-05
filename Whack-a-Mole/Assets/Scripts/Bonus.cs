using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField] private float speed; // скорость передвижения
    [SerializeField] private bool cat; // определяют вид бонуса
    [SerializeField] private bool sheep;
    [SerializeField] private bool duck;
    private Animator animator;
    [SerializeField] private AnimationClip[] clips; // основные анимации
    [HideInInspector] public bool alreadyClickedBonus = false; // предотвращает двойное нажатие
    private int scoreCheckBonus; // Уровень очков после которого увеличивается скорость
    private void Start()
    {
        animator = GetComponent<Animator>();
        scoreCheckBonus = 50;// задаем начальный уровень
    }
    private void FixedUpdate()
    {
        if (gameObject.transform.position.x > 13) // Отключаем бонус при его игнорировании
            gameObject.SetActive(false);
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        IncreaseOfSpeed(UIManager.instance.pointsScrore, ref scoreCheckBonus, ref speed);
    }
    /// <summary>
    /// Логика поведения бонуса после подбора
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetBonus()
    {
        alreadyClickedBonus = true;
        SoundManager.instance.PlaySound(SoundManager.instance.bonus);
        animator.SetTrigger("Tap");
        yield return new WaitForSeconds(clips[1].length);
        if (cat)
            UIManager.instance.catScore++;
        if (sheep)
            UIManager.instance.sheepScore++;
        if (duck)
            UIManager.instance.duckScore++;
        gameObject.SetActive(false);
        alreadyClickedBonus = false;
    }
    /// <summary>
    /// Повышает скорость в зависимости от набранных очков
    /// </summary>
    /// <param name="score">Набранные очки</param>
    /// <param name="scoreCheck">Уровень очков(после которого увеличивается скорость)</param>
    /// <param name="speed">Изменяемая скорость</param>
    private void IncreaseOfSpeed(int score, ref int scoreCheck, ref float speed)
    {
        if(speed < 4)
        {
            if (score > scoreCheck)
            {
                speed += 0.5f;
                scoreCheck += 50;
            }
        }
    }
}
