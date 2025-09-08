using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class HScrollController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scroll;
    public Pieces currentClickScroll;
    //public List<Piece> lstScroll;
    public Transform parentElement;
    public Transform parentDrag;

    public Camera camera;

    public GridLayoutGroup gridLayoutGroup;
    public ContentSizeFitter contentSizeFitter;

    //private bool isScroll;
    public bool isCanDrag = false;
    Vector2 startPoint;

    public void Init()
    {
        camera = Camera.main;
    }
    bool startDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //isScroll = false;
        startPoint = eventData.position;
    }
    void StartDrag()
    {
        startDrag = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //isScroll = true;
        currentClickScroll = null;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 directionDrag = eventData.position - startPoint;
        if (Vector2.SqrMagnitude(directionDrag) > 0.5f)
        {
            if (scroll.enabled == false)
                return;
            if (currentClickScroll == null)
                return;
            //if (isScroll)
            //    return;
            if (isCanDrag == false)
                return;
            float angleDrag = Vector2.Angle(directionDrag.normalized, Vector2.up);
            float tempAngle = 70;
            if (/*directionDrag.x < 0 ?*/ angleDrag < tempAngle /*: angleDrag > tempAngle*/ && isCanDrag == true)
            {
                currentClickScroll.ActiveDrag(true);
                scroll.enabled = false;
                //isScroll = false;
                GamePlayController.Instance.gameScene.blockRaycast.SetActive(true);
            }
            else
            {
                GamePlayController.Instance.gameScene.blockRaycast.SetActive(false);
                currentClickScroll.ActiveDrag(false);
            }
        }
    }

    public void ActiveDrag(int index)
    {
        var level = GamePlayController.Instance.playerContain.levelData;
        //for (int i = 0; i < level.pieces.Count; i++)
        //{
        //    level.pieces[i].transform.DOKill();
        //}
        for (int i = index + 1; i < level.pieces.Count; i++)
        {
            level.pieces[i].transform.DOLocalMoveX(level.pieces[i - 1].firstPos.x, 0.3f);
        }
    }

    public void ReturnScroll(int index, UnityAction actionReturnDone)
    {
        var level = GamePlayController.Instance.playerContain.levelData;
        //for (int i = 0; i < level.pieces.Count; i++)
        //{
        //    level.pieces[i].transform.DOKill();
        //}
        if (index == level.pieces.Count - 1)
        {
            actionReturnDone();
            return;
        }
        else
        {
            for (int i = index + 1; i < level.pieces.Count; i++)
            {
                level.pieces[i].transform.DOLocalMoveX(level.pieces[i].firstPos.x, 0.3f);
                actionReturnDone();
            }
        }


    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //isScroll = false;
            currentClickScroll = null;
        }
    }

}
