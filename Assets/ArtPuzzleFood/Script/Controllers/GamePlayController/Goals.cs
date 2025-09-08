using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class Goals : MonoBehaviour
{
    public int id;
    public bool isComplete;
    public Image thumnails;
    public Grass grass;
    [HideInInspector] public float fadeValue = 0f;
    [HideInInspector] public float colorValue = 0f;
    int fadePropertyID;
    private Coroutine goalsPostCoroutine;
    public Sprite target;
    public Pieces pieces;
    public void Init ( )
    {
     
    }    
    public void SetTarget()
    {
       target = thumnails.sprite;
    }

    public void CheckComplete()
    {
        thumnails.sprite = target; 
        GamePlayController.Instance.gameScene.barPercent.HandleSubtract();
        thumnails.material = GamePlayController.Instance.playerContain._colorChange;
        ColorChange();
        StartCoroutine(CheckViewIndex());
    }
    private IEnumerator CheckViewIndex()
    {
        GamePlayController.Instance.playerContain.hScrollController.currentClickScroll = null;
        yield return new WaitForSeconds(0.4f);
        GamePlayController.Instance.playerContain.hScrollController.currentClickScroll = null;
        var gamePlayControl = GamePlayController.Instance;
        List<Pieces> remainPieces = new List<Pieces>();
        for (int i = 0; i < gamePlayControl.playerContain.levelData.pieces.Count; i++)
        {
            if (!gamePlayControl.playerContain.levelData.pieces[i].isDone)
            {
                remainPieces.Add(gamePlayControl.playerContain.levelData.pieces[i]);
            }
        }

        foreach (Pieces piece in remainPieces)
        {
            GamePlayController.Instance.playerContain.hScrollController.currentClickScroll = null;
            piece.InitState(piece.transform.localPosition, piece.gameObject.transform.GetSiblingIndex());
            gamePlayControl.playerContain.hScrollController.gridLayoutGroup.enabled = true;
            gamePlayControl.playerContain.hScrollController.contentSizeFitter.enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        //GamePlayController.Instance.countPicesShowInter++;
        //if (!DataLevelSave.GetCompleteLevel(gamePlayControl.level.bundle.realLevel) && GamePlayController.Instance.countPicesShowInter >= RemoteConfigController.GetIntConfig(FirebaseConfig.COUNT_PICES_DONE_SHOW_INTER, 3))
        //    GameController.Instance.admobAds.ShowInterstitial(false, "in_game", isShowBreakAds: true);
        GamePlayController.Instance.playerContain.hScrollController.currentClickScroll = null;
        gamePlayControl.playerContain.hScrollController.gridLayoutGroup.enabled = false;
        gamePlayControl.playerContain.hScrollController.contentSizeFitter.enabled = false;

    }

    public void ColorChange()
    {
        thumnails.color = Color.white;
        var gamePlayControl = GamePlayController.Instance;
        Sequence s = DOTween.Sequence();
        s.Append(thumnails.GetComponent<RectTransform>().DOScale(new Vector3(1.07f, 1.07f, 1), .175f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            thumnails.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1), .175f).SetEase(Ease.InQuad);
        }));
        s.Join(DOTween.To(() => colorValue, x => colorValue = x, 0.55f, 0.325f).OnUpdate(() =>
        {
            thumnails.material.SetFloat("_StrongTintFade", colorValue);
        }).OnComplete(() =>
        {
            DOTween.To(() => colorValue, x => colorValue = x, 0, 0.325f).OnUpdate(() =>
            {
                thumnails.material.SetFloat("_StrongTintFade", colorValue);
            });
        }));
        s.OnComplete(() =>
        {
            //for (int i = 0; i < gamePlayControl.level.lsDataGoalsPost.Count; i++)
            //{
            //    if (gamePlayControl.level.lsDataGoalsPost[i].id == this.id)
            //    {
            //for (int j = 0; j < GameAssets.Instance._shaderChange.Count; j++)
            //{
            //    if (this.id == int.Parse(GameAssets.Instance._shaderChange[j].name))
            //    {
            this.thumnails.material = new Material(GamePlayController.Instance.playerContain._shaderChange);
            fadePropertyID = Shader.PropertyToID("_FullAlphaDissolveFade");
            fadeValue = 0;
            if (goalsPostCoroutine != null)
            {
                StopCoroutine(goalsPostCoroutine);
                goalsPostCoroutine = null;
            }
            goalsPostCoroutine = StartCoroutine(ShaderChange());
            //if (GamePlayController.Instance.state == StateGame.Playing)
            //{
            //    goalsPostCoroutine = StartCoroutine(ShaderChange());
            //}
            //    }
            //}

            //    }
            //}

        });
    }
    public IEnumerator ShaderChange()
    {
        float sign = (fadeValue - 1 == 0) ? -1 : 1;
        GamePlayController.Instance.gameScene.blockRaycast.SetActive(false);
       // GamePlayController.Instance.gameScene.controller.isCanDrag = true;
        while (true)
        {
            /*GamePlayController.Instance.gameScene.blockRaycast.SetActive(true);
            GamePlayController.Instance.gameScene.controller.isCanDrag = false;*/
         //   GamePlayController.Instance.gameScene.controller.currentClickScroll = null;
            fadeValue += sign * 0.025f;
            if (fadeValue >= 1)
            {

                StopCoroutine(goalsPostCoroutine);
                break;
            }
            thumnails.material.SetFloat(fadePropertyID, fadeValue);
         
            yield return null;
        }
    }
}
