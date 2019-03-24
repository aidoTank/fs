using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Test
{
	public static Vector3 ToVector3(this Vector2 vec)
	{
		return new Vector3(vec.x, 0, vec.y);
	}

	public static Vector2 ToVector2(this Vector3 vec)
	{
		return new Vector2(vec.x, vec.z);
	}
}

public enum eColliderType
{
	None,
	Circle,
	Polygon,
}

public class Collider
{
	public eColliderType type;
	public Vector2 c;

	public bool isObstacle = false;

	public eColliderType GetColliderType()
	{
		return type;
	}

	public Transform m_trans;
	public void AddOffsetPos(Vector2 oPos)
	{
		c += oPos;
	}

	public virtual void Update()
	{
		m_trans.position = c.ToVector3();
	}

    public virtual void Draw()
    {

    }
}

public class Polygon : Collider
{
	public Polygon()
	{
		type = eColliderType.Polygon;
	}



	private List<Vector2> m_list = new List<Vector2>();
	public List<Vector2> edgesVecList;
	public List<Vector2> wPosList;

	public void Add(Vector2 point)
	{
		m_list.Add(point);
	}

	public void InitDdgesVecList()
	{
		wPosList = LocalToWorldBound();
		edgesVecList = new List<Vector2>();
		for(int i = 0; i < wPosList.Count; i ++)
		{
			// int pointIndex1 = i;
			// int pointIndex2 = (i + 1) % wPosList.Count;
			// Vector2 vec = wPosList[pointIndex2] - wPosList[pointIndex1];
			// edgesVecList.Add(vec);
			for(int j = 0; j < wPosList.Count; j ++)
			{
				Vector2 vec = wPosList[j] - wPosList[i];
				edgesVecList.Add(vec);
			}
		}
	}

	public List<Vector2> LocalToWorldBound()
	{
		List<Vector2> list = new List<Vector2>();
		for(int i = 0 ; i < m_list.Count; i ++)
		{
			list.Add(LocalToWorld(m_list[i]));
		}
		return list;
	}

    private Vector2 LocalToWorld(Vector2 v)
    {
		float angle = m_trans.localEulerAngles.y;
        float rad = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        float newX = v.x * cos + v.y * sin;
        float newY = v.x * -sin + v.y * cos;
        return new Vector2(c.x + newX, c.y + newY);
    }

	public override void Draw()
	{
  //      List<Vector2> worldBound = LocalToWorldBound();
  //      UnityEditor.Handles.color = Color.white;
  //      for (int i = 0; i < worldBound.Count; i++)
		//{
		//	Vector2 i1 = worldBound[(i + 1) % worldBound.Count];
		//	Gizmos.DrawLine(new Vector3(worldBound[i].x, 0, worldBound[i].y), 
		//	new Vector3(i1.x, 0, i1.y));
		//}
	}
}

public class Circle : Collider
{
	public Circle()
	{
		type = eColliderType.Circle;
	}

	public float r;

    private float time;
    private Vector2 dir;
    public override void Update()
    {
        base.Update();
        time += Time.deltaTime;
        if(time > 2)
        {
            dir = new Vector2(UnityEngine.Random.Range(-1.0f, 2.0f), UnityEngine.Random.Range(-1.0f, 2.0f));
            time = 0;
        }
        AddOffsetPos(dir * Time.deltaTime * 5);
    }
}

public class CustomCollider : MonoBehaviour
{
	public Transform o1;
	public Transform[] oList;
	public Transform[] pList;

	private Circle c1;
	private List<Collider> list = new List<Collider>();

	void Start () {
		c1 = new Circle();
		c1.m_trans = o1;
		c1.c = o1.position.ToVector2();
		c1.r = GetComponent<SphereCollider>().radius;
        list.Add(c1);

        for (int i = 0; i < oList.Length; i++)
        {
            Circle c2 = new Circle();
            c2.m_trans = oList[i];
            c2.c = oList[i].position.ToVector2();
            c2.r = GetComponent<SphereCollider>().radius;
            //c2.isObstacle = true;
            list.Add(c2);
        }

 
        for (int i = 0; i< pList.Length; i ++)
        {
            Polygon polygon = new Polygon();
            polygon.isObstacle = true;
            polygon.m_trans = pList[i];
            polygon.c = pList[i].position.ToVector2();
            polygon.Add(new Vector2(-6, -5));
            polygon.Add(new Vector2(-6, 5));
            polygon.Add(new Vector2(6, 5));
            polygon.Add(new Vector2(6, -5));
            polygon.InitDdgesVecList();
            list.Add(polygon);
        }
	}
	
	void Update () 
	{
		for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
				Check(list[i], list[j]);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i].Update();
        }

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		Vector2 moveVec = new Vector2(h, v);
		moveVec.Normalize();
		if (moveVec != Vector2.zero)
		{
			c1.c += moveVec * Time.deltaTime * 5.0f;
		}
    }



	public void OnDrawGizmos()
	{
        for(int i = 0; i < list.Count; i ++)
        {
            list[i].Draw();
        }
	}
	
	public void Check(Collider a, Collider b)
	{
		eColliderType type1 = a.GetColliderType();
        eColliderType type2 = b.GetColliderType();
		switch(type1)
		{
			case eColliderType.Circle:
				switch(type2)
				{
					case eColliderType.Circle:
						CheckCircleAndCircle(a as Circle, b as Circle);
					break;
					case eColliderType.Polygon:
						CheckPolygonAndCircle(b as Polygon, a as Circle);
					break;
				}
			break;
			case eColliderType.Polygon:
				switch(type2)
				{
					case eColliderType.Circle:
						//CheckPolygonAndCircle(a as Polygon, b as Circle);
					break;
					case eColliderType.Polygon:
						//CheckPolygonAndCircle(b as Polygon, a as Circle);
					break;
				}
			break;
		}
	}

	/// <summary>
	/// 1.获取a到b向量的方向
	/// 2.获取a在此方向上的投影点
	/// 3.获取a左右为圆半径的距离，组成的直线，为a的投影线
	/// 4.获取b的投影线
	/// 5.对比2条投影线是否相交
	/// 6.如果相交，获取a是否在左边，以及相交的距离
	/// 7.根据ab方向，偏移量，可以知道具体偏移向量
	/// 8.根据具体偏移向量和是否a在左边，来计算推开的距离
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	private bool CheckCircleAndCircle(Circle a, Circle b)
	{
		Vector2 axis = a.c - b.c;
		axis.Normalize();

		float projPoint = Vector2.Dot(a.c, axis);
		float min = projPoint - a.r;
		float max = projPoint + a.r;
		Vector2 projA = new Vector2(min, max);

		projPoint = Vector2.Dot(b.c, axis);
		min = projPoint - b.r;
		max = projPoint + b.r;
		Vector2 projB = new Vector2(min, max);

		if(CheckLine(projA, projB))
		{
			bool bAtLeft = false;
			float offset = 0;
			SetPushVec(projA, projB, ref bAtLeft, ref offset);

			Push(a, b, offset, axis, bAtLeft);
			return true;
		}
		return false;
	}

	/// <summary>
	/// 1.获取所有点的世界坐标
	/// 2.获取每条边的向量
	/// 3.遍历每条边的向量，获取它的垂直向量，也就是投影轴
	/// 4.通过当前多边形的某一边的投影轴，计算圆在这个投影轴上的投影线段
	/// 5.通过当前多边形的某一边的投影轴，计算当前多边形在这投影轴上的线段，遍历所有点，取最小和最大
	/// 6.对比多边形投影线段，和圆投影线段是否相交，如果不相交则不碰撞，退出循环
	/// 7.如果相交，获取a是否在左边，以及相交的距离
	/// 8.继续循环下一个边向量，取最小的相交距离，因为最小的相交才是真正的相交距离
	/// </summary>
	/// <returns></returns>
	private bool CheckPolygonAndCircle(Polygon a, Circle b)
	{
		bool bInit = false;
		bool bAtLeft  = true;
		Vector2 offsetVec = Vector2.zero;
		float offsetLen = 0;

		List<Vector2> wPosList = a.wPosList;
		List<Vector2> edgesVecList  = a.edgesVecList;

		for(int i = 0; i < edgesVecList.Count; i ++)
		{
			Vector2 axis = edgesVecList[i];
			// 获取投影轴
			axis = GetN(axis);
			axis.Normalize();

			// 获取当前投影轴上，多边形的投影线
			Vector2 curPoint = wPosList[0];
			float minProj = Vector2.Dot(curPoint, axis);
			float maxPorj = minProj;
			for(int p = 1; p < wPosList.Count; p ++)
			{
				curPoint = wPosList[p];
				float curPorj = Vector2.Dot(curPoint, axis);
				if(curPorj < minProj)
					minProj = curPorj;
				if(curPorj > maxPorj)
					maxPorj = curPorj;
			}
			Vector2 projA = new Vector2(minProj, maxPorj);
			
			// 求圆的投影点和线
			float projPont = Vector2.Dot(b.c, axis);
			float min = projPont - b.r;
			float max = projPont + b.r;
			Vector2 projB = new Vector2(min, max);

			if(!CheckLine(projA, projB))
			{
				return false;
			}
			else
			{
				bool bAtLeftTemp = false;
				float offsetLenTemp = 0;
				SetPushVec(projA, projB, ref bAtLeftTemp, ref offsetLenTemp);

				if(!bInit)
				{
					bAtLeft = bAtLeftTemp;
					offsetLen = offsetLenTemp;
					offsetVec = axis;
					bInit = true;
				}
				else
				{
					// 如果不是第一次，那么就得获取最小的相交距离
					if(offsetLenTemp < offsetLen)
					{
						bAtLeft = bAtLeftTemp;
						offsetLen = offsetLenTemp;
						offsetVec = axis;
					}
				}
			}
		}
		Push(a, b, offsetLen, offsetVec, bAtLeft);
		return true;
	}

	/// <summary>
	/// 法向量通过 x1x2+y1y2 = 0 可得：(-B,A) 或 (B,-A)。
	/// </summary>
	private Vector2 GetN(Vector2 vec)
	{
		if(vec.y == 0)
		{
			return new Vector2(0, 1);
		}
		return new Vector2(-vec.y, vec.x);
	}

	private bool CheckLine(Vector2 a, Vector2 b)
	{
		if(a.y < b.x || a.x > b.y)
		{
			return false;
		}
		return true;
	}

	public void SetPushVec(Vector2 projA, Vector2 projB, ref bool bAtLeft, ref float offsetLen)
	{
		if(projA.x == projB.x)
		{
			if(projA.y <= projB.y)
			{
				bAtLeft = true;
				offsetLen = projA.y - projA.x;
			}
			else
			{
				bAtLeft = false;
				offsetLen = projB.y - projA.x;
			}
		}
		else if(projA.x < projB.x)
		{
			bAtLeft = true;
			offsetLen = projA.y - projB.x;
		}
		else
		{
			bAtLeft = false;
			offsetLen = projB.y - projA.x;
		}
	}

	public void Push(Collider a, Collider b, float offsetLen, Vector2 offsetVect, bool bAtLeft)
	{	
		Vector2 offsetPos = offsetLen * offsetVect;
		//Debug.Log("offsetPos:" + offsetPos);

		//if(!a.isObstacle && !b.isObstacle)
		//{
		//	if(bAtLeft)
		//	{
		//		b.AddOffsetPos(offsetPos);
		//	}
		//	else
		//	{
		//		b.AddOffsetPos(-offsetPos);
		//	}
		//}
		// A是障碍
		if(a.isObstacle && !b.isObstacle)
		{
			if(bAtLeft)
			{
				b.AddOffsetPos(offsetPos);
			}
			else
			{
				b.AddOffsetPos(-offsetPos);
			}
		}
		else if(!a.isObstacle && b.isObstacle)
		{
			if(bAtLeft)
			{
				a.AddOffsetPos(-offsetPos);
			}
			else
			{
				a.AddOffsetPos(offsetPos);
			}
		}
	}
}
