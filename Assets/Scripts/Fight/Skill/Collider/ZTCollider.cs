using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollBase : IMove
{
    public enum PosType
    {
        SKILL,          //技能目标点
        SKILL_ROTATE,   //技能目标点 + 技能旋转旋转
        PLAYER,         //玩家
    }
    public enum ColType
    {
        CIRCLE,     //圆
        RECTANGLE,       //矩形
        SECTOR,     //扇形
        TARGET,     //指定目标碰撞
    }

    public ColType ColliderType = ColType.CIRCLE;
    protected float _x;
    protected float _y; //对应场景z
    protected float _rotate;    //对应3d坐标系 y轴旋转(碰撞计算 取反)

    public virtual float x
    {
        set
        {
            _x = value;
        }
        get
        {
            return _x;
        }
    }

    public virtual float y
    {
        set
        {
            _y = value;
        }
        get
        {
            return _y;
        }
    }

    public virtual float rotate
    {
        set
        {
            _rotate = value;
        }
        get
        {
            return _rotate;
        }
    }

    public CollBase(float tx, float ty, float tRotate)
    {
        _x = tx;
        _y = ty;
        _rotate = tRotate;
    }

    public Vector2 GetV2Pos()
    {
        return new Vector2(x, y);
    }

    //碰撞计算 取的旋转
    public float GetCalRotate()
    {
        return -_rotate;
    }

    //移动接口实现
    public Vector3 MovePos { get { return new Vector3(x, 0, y); } set { x = value.x; y = value.z; } }
}

public class CollRadius : CollBase
{
    public float Radius;

    public CollRadius(float tx, float ty, float tRotate, float tRaduis)
        : base(tx, ty, tRotate)
    {
        Radius = tRaduis;
        ColliderType = ColType.CIRCLE;
    }
}

public class CollRectange : CollBase
{
    public override float x
    {
        set
        {
            _x = value;
            _centerPoint.x = _x;
        }
        get
        {
            return _x;
        }
    }

    public override float y
    {
        set
        {
            _y = value;
            _centerPoint.y = _y;
        }
        get
        {
            return _y;
        }
    }

    public override float rotate
    {
        set
        {
            _rotate = value;
            _axes = new Vector2[] { new Vector2(Mathf.Cos(GetCalRotate()), Mathf.Sin(GetCalRotate())), new Vector2(-1 * Mathf.Sin(GetCalRotate()), Mathf.Cos(GetCalRotate())) };
        }
        get
        {
            return _rotate;
        }
    }

    protected float _width;
    protected float _height;

    public float width
    {
        set
        {
            _width = value;
            _extents.x = _width / 2;
        }
        get
        {
            return _width;
        }
    }

    public float height
    {
        set
        {
            _height = value;
            _extents.y = _height / 2;
        }
        get
        {
            return _height;
        }
    }
    //OBB包围盒
    private Vector2 _centerPoint;
    public Vector2 centerPoint
    {
        get
        {
            return _centerPoint;
        }
    }
    private Vector2 _extents;
    public Vector2 extents{
        get {
            return _extents;
        }
    }

    private Vector2[] _axes;
    public Vector2[] axes
    {
        get
        {
            return _axes;
        }
    }

    public CollRectange(float tx, float ty, float tRotate, float w, float h)
        : base(tx, ty, tRotate)
    {
        x = tx;
        y = ty;
        _width = w;
        _height = h;
        ColliderType = ColType.RECTANGLE;

        _centerPoint = new Vector2(x, y);
        _extents = new Vector2(_width / 2, _height / 2);
        _axes = new Vector2[] { new Vector2(Mathf.Cos(GetCalRotate()), Mathf.Sin(GetCalRotate())), new Vector2(-1 * Mathf.Sin(GetCalRotate()), Mathf.Cos(GetCalRotate())) };
    }

    public float GetProjectionRadius(Vector2 axis)
    {
        return this.extents[0] * Mathf.Abs(Vector2.Dot(axis,this.axes[0])) + this.extents[1] * Mathf.Abs(Vector2.Dot(axis,this.axes[1]));

    }

    public float GetLength()
    {
        return width > height ? width * 0.5f : height * 0.5f;
    }
}

public class CollSector : CollBase
{
    public float inCircle;
    public float outCircle;
    public float angle = 1;
    public CollSector(float tx, float ty, float tRotate, float tIn, float tOut, float tAngle = 1)
        : base(tx, ty, tRotate)
    {
        inCircle = tIn;
        outCircle = tOut;
        angle = tAngle;
        ColliderType = ColType.SECTOR;
    }

}

public class CollTarget : CollBase
{
    public CollTarget(float tx, float ty, float tRotate, int battleId)
        : base(tx, ty, tRotate)
    {
        ColliderType = ColType.TARGET;
    }

}


public class ZTCollider
{
    public static bool CheckCollision(CollBase CollA, CollBase CollB)
    {
        //GameTool.Log("ZTCollider:CheckCollision",CollA.GetV2Pos(), CollB.GetV2Pos());
        if (CollA.ColliderType == CollBase.ColType.CIRCLE && CollB.ColliderType == CollBase.ColType.CIRCLE)
        {
            return RadiusToRadius(CollA as CollRadius, CollB as CollRadius);
        }

        if (CollA.ColliderType == CollBase.ColType.RECTANGLE && CollB.ColliderType == CollBase.ColType.RECTANGLE)
        {
            return RectangleToRectangle(CollA as CollRectange, CollB as CollRectange);
        }

        if (CollA.ColliderType == CollBase.ColType.SECTOR && CollB.ColliderType == CollBase.ColType.SECTOR)
        {
            return SectorToSector(CollA as CollSector, CollB as CollSector);
        }

        //圆与其他碰撞
        if (CollA.ColliderType == CollBase.ColType.CIRCLE || CollB.ColliderType == CollBase.ColType.CIRCLE)
        {
            CollBase tmpCollA = CollA.ColliderType == CollBase.ColType.CIRCLE ? CollA : CollB;
            CollBase tmpCollB = CollA.ColliderType == CollBase.ColType.CIRCLE ? CollB : CollA;
            if (tmpCollB.ColliderType == CollBase.ColType.RECTANGLE)
            {
                return RadiusToRectangle(tmpCollA as CollRadius, tmpCollB as CollRectange);
            }
            return RadiusToSector(tmpCollA as CollRadius, tmpCollB as CollSector);
        }

        //矩形与扇形碰撞
        CollBase tmpCollC = CollA.ColliderType == CollBase.ColType.RECTANGLE ? CollA : CollB;
        CollBase tmpCollD = CollA.ColliderType == CollBase.ColType.RECTANGLE ? CollB : CollA;
        return RectangleToSector(tmpCollC as CollRectange, tmpCollD as CollSector);
    }

    //圆与圆碰撞
    public static bool RadiusToRadius(CollRadius CollA, CollRadius CollB)
    {
        return Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos()) <= (CollA.Radius + CollB.Radius);
    }

    //圆与矩形
    public static bool RadiusToRectangle(CollRadius CollA, CollRectange CollB)
    {
        //长轴距离过滤
        if (Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos()) > CollB.GetLength() + CollA.Radius)
        {
            return false;
        }

        //获得圆相对矩形中心的相对坐标
        float newRx = CollA.x - CollB.x;
        float newRy = CollA.y - CollB.y;
        if (Mathf.RoundToInt(CollB.GetCalRotate()) != 0)
        {
            //计算最新角度（与X轴的角度），同数学X Y轴 以矩形中心为坐标中心
            float rotate = Mathf.Atan((CollB.y - CollA.y) / (CollB.x - CollA.x)) * Mathf.Rad2Deg;
            float newrot = rotate - CollB.GetCalRotate(); //减去矩形旋转角度 与矩形同时旋转对应的角度

            float distance = Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos());
            //圆相对矩形的 x y
            newRx = Mathf.Cos(newrot * Mathf.Deg2Rad) * distance;
            newRy = Mathf.Sin(newrot * Mathf.Deg2Rad) * distance;
        }


        //矩形 与圆 相交公式 获得在矩形 x y范围内 与圆心最近的点
        float dx = Mathf.Min(newRx, CollB.width * 0.5f);
        float dx1 = Mathf.Max(dx, -CollB.width * 0.5f);
        float dy = Mathf.Min(newRy, CollB.height * 0.5f);
        float dy1 = Mathf.Max(dy, -CollB.height * 0.5f);
        return (dx1 - newRx) * (dx1 - newRx) + (dy1 - newRy) * (dy1 - newRy) <= CollA.Radius * CollA.Radius;
    }


    //圆与扇形 (暂时不做扇形内圆过滤 如有必需 多算一次小扇形是否相交)
    public static bool RadiusToSector(CollRadius CollA, CollSector CollB)
    {
        //长轴距离过滤
        if (Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos()) > CollB.outCircle + CollA.Radius)
        {
            return false;
        }

        //扇形最远点 即中轴与x轴夹角 （不做旋转时 扇形对称分布 夹角0）
        float angle = CollB.GetCalRotate();
        float cosA = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sinA = Mathf.Sin(angle * Mathf.Deg2Rad);
        //计算最远点坐标
        float x2 = CollB.x + cosA * CollB.outCircle;
        float y2 = CollB.y + sinA * CollB.outCircle;
        float theta = CollB.angle * Mathf.Deg2Rad;
        return IsCircleIntersectFan(CollA.x, CollA.y, CollA.Radius, CollB.x, CollB.y, x2, y2, theta);
    }



    //矩形与矩形
    public static bool RectangleToRectangle(CollRectange CollA, CollRectange CollB)
    {
        //长轴距离过滤
        if (Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos()) > CollB.GetLength() + CollA.GetLength())
        {
            return false;
        }

        Vector2 nv = CollA.centerPoint - CollB.centerPoint;
        Vector2 axisA1 = CollA.axes[0];
        if (CollA.GetProjectionRadius(axisA1) + CollB.GetProjectionRadius(axisA1) <= Mathf.Abs(Vector2.Dot(nv, axisA1))) return false;
        Vector2 axisA2 = CollA.axes[1];
        if (CollA.GetProjectionRadius(axisA2) + CollB.GetProjectionRadius(axisA2) <= Mathf.Abs(Vector2.Dot(nv, axisA2))) return false;
        Vector2 axisB1 = CollB.axes[0];
        if (CollA.GetProjectionRadius(axisB1) + CollB.GetProjectionRadius(axisB1) <= Mathf.Abs(Vector2.Dot(nv, axisB1))) return false;
        Vector2 axisB2 = CollB.axes[1];
        if (CollA.GetProjectionRadius(axisB2) + CollB.GetProjectionRadius(axisB2) <= Mathf.Abs(Vector2.Dot(nv, axisB2))) return false;
        return true;
    }

    //矩形与扇形(分离轴 三角形碰撞)
    public static bool RectangleToSector(CollRectange CollA, CollSector CollB)
    {
        //长轴距离过滤
        if (Vector2.Distance(CollA.GetV2Pos(), CollB.GetV2Pos()) > CollB.outCircle + CollA.GetLength())
        {
            return false;
        }

        //扇形两个顶点
        float angle = CollB.angle + CollB.GetCalRotate();
        float x1 = CollB.x + Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
        float y1 = CollB.y + Mathf.Sin(angle * 0.5f * Mathf.Deg2Rad);

        float x2 = CollB.x + Mathf.Cos(-angle * 0.5f * Mathf.Deg2Rad);
        float y2 = CollB.y + Mathf.Sin(-angle * 0.5f * Mathf.Deg2Rad);

       
        //三角形三个顶点
        Vector2[] secPos = new Vector2[] { CollB.GetV2Pos(), new Vector2(x1, y1), new Vector2(x2, y2) };
        //三角形三条中轴
        //Vector2[] secAxies = new Vector2[] { GetV2Nor(secPos[0] - secPos[1]), GetV2Nor(secPos[0] - secPos[2]), GetV2Nor(secPos[1] - secPos[2]) };
        //矩形四个顶点
        float rectR = CollA.extents.magnitude;
        float rotate = Mathf.Atan((CollA.width * 0.5f) / (CollA.height * 0.5f)) * Mathf.Rad2Deg + CollA.GetCalRotate();
        float x = Mathf.Cos(rotate) * rectR;
        float y = Mathf.Sin(rotate) * rectR;
        Vector2[] rectPos = new Vector2[] {
            new Vector2(CollA.x + x,CollA.y + y),
            new Vector2(CollA.x + x,CollA.y - y),
            new Vector2(CollA.x - x,CollA.y + y),
            new Vector2(CollA.x - x,CollA.y - y)
        };
         //矩形中轴
        //Vector2[] rectAxies = new Vector2[] { GetV2Nor(secPos[0] - secPos[1]), GetV2Nor(secPos[0] - secPos[3]), GetV2Nor(secPos[1] - secPos[2]),GetV2Nor(secPos[2] - secPos[3]) };

        Vector2[] axises = new Vector2[]{
            GetV2Nor(secPos[0] - secPos[1]), GetV2Nor(secPos[0] - secPos[2]), GetV2Nor(secPos[1] - secPos[2]),
            GetV2Nor(secPos[0] - secPos[1]), GetV2Nor(secPos[0] - secPos[3]), GetV2Nor(secPos[1] - secPos[2]),GetV2Nor(secPos[2] - secPos[3])
        };

		for (int i = 0, len = axises.Length; i < len; i++) {
			var axis = axises[i];

            Vector2 proA = GetProjection(axis, secPos);
            Vector2 proB = GetProjection(axis, rectPos);

            if (IsOverlay(proA, proB))
            {
                return false;
            }
		}
        
        return true;
    }
    //获取法向量
    public static Vector2 GetV2Nor(Vector2 pos)
    {
        return new Vector2(-pos.y, pos.x);
    }
    //获取投影
    public static Vector2 GetProjection(Vector2 axis,Vector2[] vecPos)
    {
        float min = 0;
        float max = 0;
		for (int i = 0, l = vecPos.Length; i < l; i++) {
            Vector2 p = vecPos[i];
			var pro = Vector2.Dot(p, axis) / axis.magnitude;
			if (pro < min) {
				min = pro;
			}
			if (pro > max) {
				max = pro;
			}
		}
		return new Vector2(min,max);
    }
    //是否有空隙
    public static bool IsOverlay (Vector2 proA,Vector2 proB) {
        float x = 0;
        float y = 0;
		if (proA.x < proB.x) {
			x = proA.x;
		} else {
			x = proB.x;
		}

		if (proA.y > proB.y) {
			y = proA.y;
		} else {
			y = proB.y;
		}
		return (proA.y - proA.x) + (proB.y - proB.x) < y - x;
	}

    //扇形与扇形
    public static bool SectorToSector(CollSector CollA, CollSector CollB)
    {
        return false;
    }



    //圆与扇形
    // 圆心p(x, y), 半径r, 扇形圆心p1(x1, y1), 扇形正前方最远点p2(x2, y2), 扇形夹角弧度值theta(0,pi)  
    public static bool IsCircleIntersectFan(float x, float y, float r, float x1, float y1, float x2, float y2, float theta)
    {
        // 计算扇形正前方向量 v = p1p2  
        float vx = x2 - x1;
        float vy = y2 - y1;

        // 计算扇形半径 R = v.length()  
        float R = Mathf.Sqrt(vx * vx + vy * vy);
        // 圆不与扇形圆相交，则圆与扇形必不相交  
        if ((x - x1) * (x - x1) + (y - y1) * (y - y1) > (R + r) * (R + r))
            return false;

        if (theta < 0.00001f && theta > 3.1416f)
        {
            return false;
        }

        // 根据夹角 theta/2 计算出旋转矩阵，并将向量v乘该旋转矩阵得出扇形两边的端点p3,p4  
        float h = theta * 0.5f;
        float c = Mathf.Cos(h);
        float s = Mathf.Cos(h);
        float x3 = x1 + (vx * c - vy * s);
        float y3 = y1 + (vx * s + vy * c);
        float x4 = x1 + (vx * c + vy * s);
        float y4 = y1 + (-vx * s + vy * c);

        // 如果圆心在扇形两边夹角内，则必相交  
        float d1 = EvaluatePointToLine(x, y, x1, y1, x3, y3);
        float d2 = EvaluatePointToLine(x, y, x4, y4, x1, y1);
        if (d1 >= 0 && d2 >= 0)
            return true;

        // 如果圆与任一边相交，则必相交  
        if (IsCircleIntersectLineSeg(x, y, r, x1, y1, x3, y3))
            return true;
        if (IsCircleIntersectLineSeg(x, y, r, x1, y1, x4, y4))
            return true;

        return false;
    }

    // 圆与线段碰撞检测  
    // 圆心p(x, y), 半径r, 线段两端点p1(x1, y1)和p2(x2, y2)  
    public static bool IsCircleIntersectLineSeg(float x, float y, float r, float x1, float y1, float x2, float y2)
    {
        float vx1 = x - x1;
        float vy1 = y - y1;
        float vx2 = x2 - x1;
        float vy2 = y2 - y1;

        // len = v2.length()  
        float len = Mathf.Sqrt(vx2 * vx2 + vy2 * vy2);
        // v2.normalize()  
        vx2 /= len;
        vy2 /= len;
        // u = v1.dot(v2)  
        // u is the vector projection length of vector v1 onto vector v2.  
        float u = vx1 * vx2 + vy1 * vy2;
        // determine the nearest point on the lineseg  
        float x0 = 0;
        float y0 = 0;
        if (u <= 0)
        {
            // p is on the left of p1, so p1 is the nearest point on lineseg  
            x0 = x1;
            y0 = y1;
        }
        else if (u >= len)
        {
            // p is on the right of p2, so p2 is the nearest point on lineseg  
            x0 = x2;
            y0 = y2;
        }
        else
        {
            // p0 = p1 + v2 * u  
            // note that v2 is already normalized.  
            x0 = x1 + vx2 * u;
            y0 = y1 + vy2 * u;
        }
        return (x - x0) * (x - x0) + (y - y0) * (y - y0) <= r * r;
    }

    // 判断点P(x, y)与有向直线P1P2的关系. 小于0表示点在直线左侧，等于0表示点在直线上，大于0表示点在直线右侧  
    public static float EvaluatePointToLine(float x, float y, float x1, float y1, float x2, float y2)
    {
        float a = y2 - y1;
        float b = x1 - x2;
        float c = x2 * y1 - x1 * y2;

        if (Mathf.Abs(a) < 0.00001f || Mathf.Abs(b) < 0.00001f)
        {
            return 0;
        }

        return a * x + b * y + c;
    }
}