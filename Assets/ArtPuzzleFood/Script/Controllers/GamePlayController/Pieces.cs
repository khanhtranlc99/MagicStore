using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Purchasing;
using Sirenix.OdinInspector;
using System;
using MoreMountains.NiceVibrations;


public class Pieces : MonoBehaviour, IPointerDownHandler
{
    public int id;
    public Vector2 startPoint;
    public Vector2 startSize;
    public HScrollController controller;
    public Vector2 firstPos;
    private bool isCanDrag;
    public bool isDragging;
    public int firstIndex;
    public Image thumnail;
    public RectTransform draggedItemRect;
    public bool isDone;
    public Goals goals;
    public AudioClip clickSfx;
    public AudioClip completeSfx;


  

    public float duration = 0.5f; // thời gian tween
    public bool IsPieceVisible()
    {
        Vector3[] viewportCorners = new Vector3[4];
        GamePlayController.Instance.playerContain.viewPort.GetWorldCorners(viewportCorners);

        Vector3[] pieceCorners = new Vector3[4];
        draggedItemRect.GetWorldCorners(pieceCorners);

        // Kiểm tra xem piece có nằm trong vùng viewport không
        bool isInside =
            pieceCorners[2].y <= viewportCorners[1].y && // top của piece < top viewport
            pieceCorners[0].y >= viewportCorners[0].y && // bottom của piece > bottom viewport
            pieceCorners[0].x >= viewportCorners[0].x && // left của piece > left viewport
            pieceCorners[2].x <= viewportCorners[2].x;   // right của piece < right viewport

        return isInside;
    }

    public void ScrollPieceIntoView( Action callBack)
    {
     
        ScrollRect scrollRect = GamePlayController.Instance.playerContain.scrollView;
        RectTransform viewport = GamePlayController.Instance.playerContain.viewPort;
        RectTransform content = scrollRect.content;

        Vector3 worldPos = this.GetComponent<RectTransform>().position;
        Vector3 localPos = content.InverseTransformPoint(worldPos);

        float viewportHeight = viewport.rect.height;
        float viewportWidth = viewport.rect.width;
        float contentHeight = content.rect.height;
        float contentWidth = content.rect.width;

        float targetY = localPos.y - (viewportHeight / 2f) + (this.GetComponent<RectTransform>().rect.height / 2f);
        float normalizedY = 1f - Mathf.Clamp01(targetY / (contentHeight - viewportHeight));

        float targetX = localPos.x - (viewportWidth / 2f) + (this.GetComponent<RectTransform>().rect.width / 2f);
        float normalizedX = Mathf.Clamp01(targetX / (contentWidth - viewportWidth));

        DOTween.To(
            () => scrollRect.verticalNormalizedPosition,
            y => scrollRect.verticalNormalizedPosition = y,
            normalizedY,
            0.3f
        ).SetEase(Ease.OutQuad);

        DOTween.To(
            () => scrollRect.horizontalNormalizedPosition,
            x => scrollRect.horizontalNormalizedPosition = x,
            normalizedX,
            0.3f
        ).SetEase(Ease.OutQuad).OnComplete(delegate { callBack?.Invoke(); });
    }


 
    

    public void InitState(Vector2 firstPos, int firstIndex)
    {
        this.firstPos = firstPos;
        this.firstIndex = firstIndex;
    
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = eventData.position;
        controller.currentClickScroll = this;
        GameController.Instance.musicManager.PlayOneShot(clickSfx);
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }
    public void ActiveDrag(bool isActive)
    {
        if (isActive && this.controller.isCanDrag)
        {
            this.transform.parent = this.controller.parentDrag;
            isCanDrag = true;
            isDragging = true;
            controller.ActiveDrag(this.firstIndex);

            var level = GamePlayController.Instance.playerContain.levelData;
            for (int i = 0; i < level.lsDataGoalsPost.Count; i++)
            {
                if (this.id == level.lsDataGoalsPost[i].id)
                {
                    this.thumnail.GetComponent<RectTransform>().DOSizeDelta(new Vector2(level.lsDataGoalsPost[i].thumnails.rectTransform.sizeDelta.x, level.lsDataGoalsPost[i].thumnails.rectTransform.sizeDelta.y), 0.2f);
                }
            }

        }

    }
    public void ReturnScroll()
    {
        this.transform.parent = this.controller.parentElement;
        this.transform.SetSiblingIndex(this.firstIndex);
        this.thumnail.GetComponent<RectTransform>().DOSizeDelta(this.startSize, 0.3f);
        controller.ReturnScroll(this.firstIndex, () =>
        {
            this.transform.DOLocalMove(firstPos, 0.3f);
            controller.isCanDrag = true;
            GamePlayController.Instance.gameScene.blockRaycast.SetActive(false);
        });
    }
    void Update()
    {
        if (isDragging && controller.isCanDrag)
        {
            Vector2 localPosition = Vector2.zero;
            Vector2 sceenPoint = Input.mousePosition + 300 * Vector3.up;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(

                draggedItemRect,

                sceenPoint,

                controller.camera,

                out localPosition);


            draggedItemRect.position = Vector3.Lerp(draggedItemRect.position, draggedItemRect.TransformPoint(localPosition), 10 * Time.deltaTime);

            //for (int i = 0; i < GamePlayController.Instance.playerContain.levelData.lsDataGoalsPost.Count; i++)
            //{
            //    if (GamePlayController.Instance.playerContain.levelData.lsDataGoalsPost[i].id == this.id)
            //    {
            //       // GamePlayController.Instance.playerContain.levelData.lsDataGoalsPost[i].CheckCompletePiece();
            //    }
            //}
            CheckComplete();


        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isCanDrag && this.isDone == false)
            {
                isDragging = false;
                isCanDrag = false;
                ReturnScroll();
                controller.scroll.enabled = true;
            }
        }
    }
    private void CheckComplete()
    {
        if (goals.transform.parent.gameObject.activeSelf )
        {
            float distance = Vector3.Distance(this.transform.position, goals.transform.position);
            if (distance < 0.5f)
            {
                MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                controller.currentClickScroll = null;
                goals.CheckComplete();
                isDragging = false;
                isCanDrag = false;
                controller.scroll.enabled = true;    
                GamePlayController.Instance.playerContain.levelData.HandleFillIndex(this);
                GamePlayController.Instance.gameScene.blockRaycast.SetActive(false);              
                StartCoroutine(ResetContentSize());
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.CHECK_HAND_BOOSTER);
                GameController.Instance.musicManager.PlayOneShot(completeSfx);
                SimplePool2.Despawn(this.gameObject);



            }
        }
    }    


    private IEnumerator ResetContentSize()
    {
        controller.gridLayoutGroup.enabled = true;
        controller.contentSizeFitter.enabled = true;
        yield return new WaitForSeconds(0.1f);
        controller.gridLayoutGroup.enabled = false;
        controller.contentSizeFitter.enabled = false;
    }
}
