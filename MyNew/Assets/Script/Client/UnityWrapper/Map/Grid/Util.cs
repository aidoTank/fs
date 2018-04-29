//#define NoPooling //Disable pooling for some reason. Could be debugging or just for measuring the difference.
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Roma
{
    public class Util
    {
        //链表转数组
        public static T[] LnkLst2Arr<T>(LinkedList<T> coll)
        {
            T[] lst = new T[coll.Count];

            LinkedListNode<T> nodeNow = coll.First;//链表第一个元素
            int i = 0;
            while (nodeNow != null)
            {
                lst[i] = nodeNow.Value;
                nodeNow = nodeNow.Next;
                i++;
            }

            return lst;
        }

        public class DistanceWrapper
        {
            //
            // 摘要:
            //     The distance from the ray's origin to the impact point.
            public float distance { get; set; }
        }

        //快速排序
        public static void QuickSort(DistanceWrapper[] arr)
        {
            int i = 0;
            LinkedList<DistanceWrapper> lst = new LinkedList<DistanceWrapper>();
            for (i = 0; i < arr.Length; i++)
            {
                lst.AddLast(arr[i]);
            }

            LinkedListNode<DistanceWrapper> itCurBegin = lst.First;
            LinkedListNode<DistanceWrapper> it = itCurBegin;
            LinkedListNode<DistanceWrapper> itMax = itCurBegin;

            if(it == null)
               return;

            bool bSuccess = true;
            while (true)
            {
                LinkedListNode<DistanceWrapper> icur = it;
                it = it.Next;
                
                if (it == null)
                {
                    if (bSuccess)
                        break;

                    DistanceWrapper tmp = itCurBegin.Value;
                    itCurBegin.Value = itMax.Value;
                    itMax.Value = tmp;
                    itCurBegin = itCurBegin.Next;
                    if (itCurBegin == null)
                        break;

                    it = itCurBegin;
                    itMax = itCurBegin;
                    bSuccess = true;
                }
                else
                {
                    if (icur.Value.distance < it.Value.distance)
                    {
                        bSuccess = false;
                        DistanceWrapper tmp = icur.Value;
                        icur.Value = it.Value;
                        it.Value = tmp;
                    }

                    if (itMax.Value.distance < icur.Value.distance)
                    {
                        itMax = icur;
                    }
                }
            }

            //Debug.LogWarning("---------------");
            LinkedListNode<DistanceWrapper> nodeNow = lst.First;//链表第一个元素
            i = 0;
            while (nodeNow != null)
            {
                arr[i] = nodeNow.Value;
                //Debug.LogWarning(arr[i].distance);
                nodeNow = nodeNow.Next;
                i++;
            }
        }
    }
}