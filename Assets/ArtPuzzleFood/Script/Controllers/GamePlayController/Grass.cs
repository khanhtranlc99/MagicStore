using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Grass : MonoBehaviour
{
   public List<Goals> lsGoals;
    public CanvasGroup canvasGroup;
    public bool isDone;
    public Goals GetGoals
    {
        get
        {
            foreach (var item in lsGoals)
            {
                if (item.isComplete == false)
                {
                    return item;
                }
            }
            
            return null;
        }
    }

    public void HandleFadeIn()
    {
        foreach(var item in lsGoals)
        {
            item.thumnails.DOFade(1,1f);
        }
    }
    public bool HandleCheckDone
    {
        get
        {
          
            foreach (var item in lsGoals)
            {
                if(item.isComplete == false)
                {
                    return false;
                }
            }
            isDone = true;
            return true;
        }
  
    }
}
