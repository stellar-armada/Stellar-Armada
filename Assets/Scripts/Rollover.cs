using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649
public class Rollover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static Rollover currentRollover;

    [SerializeField] private Image image;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color rolloverColor;

    public UnityEvent OnTriggerPressed;
    public UnityEvent OnSecondaryPressed;
    public UnityEvent OnRolloverReleasedOn;

    void Awake()
    {
        image.color = defaultColor;
    }
    
    public void HandleRolloverReleasedOn()
    {
        image.color = defaultColor;
        OnRolloverReleasedOn.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentRollover = this;
        image.color = rolloverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentRollover == this) currentRollover = null;
        image.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTriggerPressed.Invoke();
    }
    
    public void HandleSecondaryButtonPressed()
    {
        OnSecondaryPressed.Invoke();
    }
}