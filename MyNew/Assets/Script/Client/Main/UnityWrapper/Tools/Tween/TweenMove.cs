//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;
//using Roma;

//public class TweenMove : MonoBehaviour
//{
//    public delegate void TweenMoveEnd(CCreature ccreture);
//    public TweenMoveEnd moveEnd;
//    public CCreature m_creature;
//    public static void BeginMove(CCreature ccreture, Vector3 targetPos, float speed, TweenMoveEnd move)
//    {
//        TweenMove tweenMove = null;
//        if (tweenMove == null)
//        {
//            GameObject obj = ccreture.GetEntity().GetObject();
//            if (obj == null) return;
//            tweenMove = obj.AddComponent<TweenMove>();

//            tweenMove.m_creature = ccreture;
//            tweenMove.moveEnd = move;

//            //ITWEEN
//            Hashtable args = new Hashtable();
//            args.Add("speed", speed);
//            args.Add("easeType", iTween.EaseType.linear);
//            args.Add("position", targetPos);
//           // args.Add("onupdate", "ItweenUpdate");
//            args.Add("oncomplete", "ItweenMoveEnd");
//            iTween.MoveTo(obj, args);
//        }
//    }

//    void ItweenMoveEnd()
//    {
//        if (moveEnd != null)
//        {
//            moveEnd(m_creature);
//            moveEnd = null;
//        }

//        Destroy(this);
//    }

//}
