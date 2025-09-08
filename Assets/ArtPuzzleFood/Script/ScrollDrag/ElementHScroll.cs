using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class ElementHScroll : MonoBehaviour, IPointerDownHandler
{
    public CanvasGroup canvasGruop;
    public Vector2 startPoint;
    private HScrollController controller;

    public Canvas canvas;

    private bool isCanDrag;
    private bool isDragging;
    private Vector3 offsetMove;

    public RectTransform draggedItemRect;
    public void Init(HScrollController controller)
    {
        this.controller = controller;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = eventData.position;
        //controller.currentClickScroll = this;
    }

    public void ActiveDrag(bool isActive)
    {
        if (isActive)
        {
           this.transform.parent = this.controller.parentDrag;
            isCanDrag = true;
            isDragging = true;
            this.transform.DOKill();
            this.transform.DOScale(1.3f, 0.25f);
        }
    }

    public void ReturnScroll()
    {
        this.transform.parent = this.controller.parentElement;
    }



    void Update()
    {
        if(isDragging)
        {
            Vector2 localPosition = Vector2.zero;
            Vector2 sceenPoint = Input.mousePosition + 300 * Vector3.up;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(

                draggedItemRect,

                sceenPoint,

                Camera.main, //this is the thing for your camera

                out localPosition);



            draggedItemRect.position = Vector3.Lerp(draggedItemRect.position, draggedItemRect.TransformPoint(localPosition), 10 * Time.deltaTime);
        } 
        
        if(Input.GetMouseButtonUp(0))
        {
            if (isCanDrag)
            {
                isDragging = false;
                isCanDrag = false;
                ReturnScroll();
                controller.scroll.enabled = true;
                this.transform.DOKill();
                this.transform.DOScale(1f, 0.25f);
            }    
        }    
    }

    public Vector3 GetPointDistanceFromObject(float distance, Vector3 direction, Vector3 fromPoint)
    {
        distance -= 1;
        //if (distance < 0)
        //    distance = 0;

        Vector3 finalDirection = direction + direction.normalized * distance;
        Vector3 targetPosition = fromPoint + finalDirection;

        return targetPosition;
    }

}
