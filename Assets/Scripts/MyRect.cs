using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;


public class MyRect
{
    private int x, y;   //左上の位置
    private int w, h;   //幅と高さ
    private MyColor myc;    //RGB値

    public int X
    {
        get { return x; }
    }

    public int Y
    {
        get { return y; }
    }

    public int W
    {
        get { return w; }
    }

    public int H
    {
        get { return h; }
    }

    public MyRect(int _x, int _y, int _w, int _h)
    {
        x = _x;
        y = _y;
        w = _w;
        h = _h;
        myc = new MyColor(255, 255, 255);
    }

    public void setColor(MyColor _myc)
    {
        myc.B = _myc.B;
        myc.G = _myc.G;
        myc.R = _myc.R;
    }

    //たぶんこれが間違っている！
    public bool isIn(int ex , int why)
    {
        if ((ex >= x && ex <= x + w) && (why >= y && why <= y + h))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int xDist(int ex, int why)
    {  //あるx軸とぶつかるか、距離を求める
        if (why < y || why > y + h)
        {
            return -1;
        }
        else
        {
            int answer = x - ex;
            if (answer < 0) return -1;
            return answer;
        }
    }

    public int yDist(int ex, int why)
    {  //あるy軸とぶつかるか、距離
        if (ex < x || ex > x + w)
        {
            return -1;
        }
        else
        {
            int answer = y - why;
            if (answer < 0) return -1;
            return answer;
        }
    }

    //間違っている可能性が高いのここまで

    public void drawMe(Mat material)
    {
        Imgproc.rectangle(material,
            new OpenCVForUnity.CoreModule.Rect(x, y, w, h),
            new Scalar(myc.R, myc.G, myc.B),
            -1);
        Imgproc.rectangle(material,
            new OpenCVForUnity.CoreModule.Rect(x, y, w, h),
            new Scalar(0, 0, 0),
            2);
    }
}

public class MyColor
{
    private byte r, g, b;
    public byte B
    {
        get
        {
            return b;
        }
        set
        {
             b = value;
        }
    }

    public byte G
    {
        get
        {
            return g;
        }
        set
        {
            g = value;
        }
    }

    public byte R
    {
        get
        {
            return r;
        }
        set
        {
            r = value;
        }
    }

    public MyColor(byte arl, byte jee, byte vee)
    {
        b = vee;

        g = jee;

        r = arl;
    }

    public bool compare(MyColor myc)
    {
        if(r == myc.R && g == myc.G && b == myc.B)
        {
            return true;
        }
        return false;
    }
}