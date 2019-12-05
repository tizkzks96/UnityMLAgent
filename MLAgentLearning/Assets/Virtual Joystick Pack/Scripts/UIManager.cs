using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{
    public GameObject joystic;
    public static FixedJoystick fixedJoystic;

    private void Start()
    {
        fixedJoystic = joystic.GetComponent<FixedJoystick>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        fixedJoystic.Drag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(joystic != null)
        {
            fixedJoystic.JoysticPosition(eventData.position);
            joystic.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        fixedJoystic.TouchUp(eventData);
        joystic.SetActive(false);
    }
}