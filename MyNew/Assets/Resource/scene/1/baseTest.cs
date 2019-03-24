using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseTest : MonoBehaviour {

	private int[] arr = new int[]{8,7,6,5,4,3,2,1};

	/// <summary>
	/// 冒泡排序 O（n2）
	/// 1.遍历长度-1次
	/// 2.每一次从最后的位置遍历到位置i，并获取最小值，升到最前位置
	/// </summary>
	private void Sort(ref int[] arr)
	{
		//bool flag;  // 是否交互过位置,如果某一次遍历没有交换位置，说明不用再排了
		for(int i = 0; i < arr.Length - 1; i++)
		{
			//flag = false;
			// for(int j = arr.Length - 1; j > i; j --)
			// {
			// 	if(arr[j] < arr[j - 1])  // 前面的大
			// 	{
			// 		int temp = arr[j - 1];
			// 		arr[j - 1] = arr[j];
			// 		arr[j] = temp;
			// 		flag = true;
			// 	}
			// }
			// if(!flag)
			// 	break;

			for(int j = 0; j < arr.Length - 1 - i; j ++)
			{
				if(arr[j] > arr[j + 1])
				{
					int temp = arr[j];
					arr[j] = arr[j + 1];
					arr[j + 1] = temp;
				}
			}
	
		}
	}

	/// <summary>
	/// 选择排序 O（n2）
	/// 从第一个开始，一个个的选择与开始元素对比
	/// 1.遍历n-1次，每次遍历后面的数组，如果有最小的数值则与第一个元素交换
	/// </summary>
	/// <param name="arr"></param>
	private void Sort1(int[] arr)
	{
		for(int i = 0 ; i < arr.Length - 1; i ++)
		{
			int minIndex = i;   // 最小数值索引
			for(int j = i + 1; j < arr.Length; j ++)
			{
				if(arr[j] < arr[minIndex])
				{
					minIndex = j;
				}
			}
			if(minIndex != i) // 当最小值的索引不是当前第一个时，则找到最小值了
			{
				// 将最小值，和当前开始值交换
				int temp = arr[minIndex];
				arr[minIndex] = arr[i];
				arr[i] = temp; 
			}
		}
	}

	/// <summary>
	/// 插入排序 O（n2）
	/// 选择一个元素，从后向前对比，再插入合适的位置
	/// 1.遍历从第一个索引开始插入，遍历插入位置之前的元素，让更小的上升
	/// </summary>
	private void Sort2(ref int[] arr)
	{
		for(int i = 0; i < arr.Length - 1; i ++)
		{
			for(int j = i + 1; j > 0; j --)
			{
				if(arr[j] < arr[j - 1])  // 当下一个数比较小时，才交换
				{
					int temp = arr[j - 1];
					arr[j - 1] = arr[j];  // 交换位置
					arr[j] = temp;
				}
				else
				{
					break;
				}
			}
		}
	}

	private void QuickSort(ref int[] a, int low, int hight)
	{
		if(low >= hight)
			return;
		
		int frist = low;
		int last = hight;
		int key = a[frist];

		while(frist < last)
		{
			while(frist < last && a[last] >= key)
			{
				last --;
			}
			a[frist] = a[last];
			while(frist < last && a[frist] <= key)
			{
				frist ++;
			}
			a[last] = a[frist];
		}
		a[frist] = key;
		QuickSort(ref a, low, frist - 1);
		QuickSort(ref a, frist + 1, hight);
	}

	void Start () {
		Sort(ref arr);
		//Sort2(ref arr);

		//QuickSort(ref arr, 0, arr.Length -1);
		for(int i = 0; i < arr.Length; i ++)
		{
			Debug.Log("=" + arr[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
