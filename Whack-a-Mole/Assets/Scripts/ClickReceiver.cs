using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickReceiver : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public Enemy enemy;
    [HideInInspector] public Bonus bonus;
    /// <summary>
    /// Передает объекту инф о том что на него кликнули
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.rawPointerPress.CompareTag("Enemy"))
        {
            enemy = eventData.rawPointerPress.GetComponent<Enemy>();
            if (!enemy.alreadyClicked)
                StartCoroutine(enemy.DelayBeforeDisableMyself());
        }
        if(eventData.rawPointerPress.CompareTag("Bonus"))
        {
            bonus = eventData.rawPointerPress.GetComponent<Bonus>();
            if(!bonus.alreadyClickedBonus)
                StartCoroutine(bonus.GetBonus());
        }
    }
}
