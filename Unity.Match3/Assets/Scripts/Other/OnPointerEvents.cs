using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPointerEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Settings")]
    public bool onPointerExit = true;

    public Action<PointerEventData> onPointerDown;
    public Action<PointerEventData> onPointerUp;
    public Action<PointerEventData> onPointerClick;

    [HideInInspector] public bool isActive = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isActive)
        {
            onPointerDown?.Invoke(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isActive)
        {
            onPointerUp?.Invoke(eventData);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (isActive && onPointerExit)
        {
            onPointerUp?.Invoke(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive)
        {
            onPointerClick?.Invoke(eventData);
        }
    }
}
