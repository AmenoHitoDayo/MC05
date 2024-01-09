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

    public bool isIn(int ex , int why)
    {
        if ((ex >= x && ex < x + w) && (why >= y && why < y + h))
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
            if(value <= 255)
            {
                b = value;
            }
            else
            {
                b = 255;
            }
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
            if(value <= 255)
            {
                g = value;
            }
            else
            {
                g = 255;
            }
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
            if(value <= 255)
            {
                r = value;
            }
            else
            {
                r = 255;
            }
        }
    }

    public MyColor(byte arl, byte jee, byte vee)
    {
        if (vee <= 255 && vee >= 0)
            b = vee;
        else if (vee > 255)
            b = 255;
        else
            b = 0;

        if (jee <= 255 && jee >= 0)
            g = jee;
        else if (jee > 255)
            g = 255;
        else
            g = 0;

        if (arl <= 255 && arl >= 0)
            r = arl;
        else if (arl > 255)
            r = 255;
        else
            r = 0;
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