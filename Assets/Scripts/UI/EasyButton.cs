using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

public class EasyButton : Button
{
    [Serializable]
    public class ButtonUpEvent : UnityEvent { }

    [FormerlySerializedAs("onUp")]
    [SerializeField]
    private ButtonUpEvent m_OnUp = new ButtonUpEvent();

    public ButtonUpEvent onUp
    {
        get { return m_OnUp; }
        set { m_OnUp = value; }
    }
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        onClick.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onUp", this);
        onUp.Invoke();
    }
}
