using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
public class BarPercent : MonoBehaviour
{
    public int sumPieces;
    public int currentNumb;
    public Text tvNumPieces;
    public Image amount;
    LevelData levelData;


    public float currentBar;
    public float totalBar;
    public AudioClip sfxComplete;
    public void Init(LevelData param)
    {
        levelData  = param;
        sumPieces = 0;
        foreach(var item in levelData.lsGrass)
        {
            foreach (var piece in item.lsGoals)
            {
                sumPieces += 1;
            }
        }
        currentNumb = sumPieces;
        tvNumPieces.text = currentNumb + "/"+ sumPieces.ToString();
        amount.fillAmount = 0;
        currentBar = 0;
     

    }    
    public void HandleSubtract()
    {
        currentNumb -= 1;
        tvNumPieces.text = currentNumb  + "/"+ sumPieces.ToString();
        HandlePlusBarGrass();
    
    }    

    public void HandleChangeBar(int paralTotal)
    {
        currentBar = 0;
        totalBar = paralTotal;
      


    }    

    public void HandlePlusBarGrass()
    {
        currentBar += 1;
        amount.DOFillAmount(currentBar / totalBar, 0.4f).OnComplete(delegate { 
           if(amount.fillAmount >= 1)
            {

                amount.fillAmount = 0;
                GameController.Instance.musicManager.PlayOneShot(sfxComplete);
            }    
        });
    }    




}
