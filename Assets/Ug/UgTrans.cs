using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgTrans
{
    public string parentName = "";
    public string name = "";
    public Vector2 pos = Vector2.zero;
    public Vector2 scale = Vector2.one;
    public float rotation = 0;
    public UgMatrix2D matrix;
    public UgMatrix2D deltaMatrix;
    public UgMatrix2D startMatrix;
    public Vector2 startPos;
    public Vector2 startScale = Vector2.one;
    public float startRotation = 0;

    public UgTrans parent = null;
    public List<UgTrans> children = new List<UgTrans>();

    public virtual void Init(UgData.TransformData data)
    {
        Debug.Log("init");
        this.name = data.name;
        this.parentName = data.parent;
        this.pos = data.pos;
        this.scale = data.scale;
        this.rotation = data.rotation;
        startPos = data.pos;
        startRotation = data.rotation;
        startScale = data.scale;
    }

    public void updateMatrix()
    {
        if (null == this.matrix) {
            //Debug.Log("updateMatrix");
            this.matrix = new UgMatrix2D();
            this.deltaMatrix = new UgMatrix2D();
            this.startMatrix = new UgMatrix2D();
        }
        if (this.rotation == 0.0f)
        {
            this.matrix.a = 1.0f;
            this.matrix.b = 0.0f;
        }
        else
        {
            this.matrix.a = (float)Math.Cos(-Math.PI * this.rotation / 180);
            this.matrix.b = (float)Math.Sin(-Math.PI * this.rotation / 180);
        }

        //if (this.skew == 0.0f)
        //{
        this.matrix.c = -this.matrix.b;
        this.matrix.d = this.matrix.a;
        //}
        //else
        //{
        //    matrix.c = -(float)Math.Sin(this.skew + this.rotation);
        //    matrix.d = (float)Math.Cos(this.skew + this.rotation);
        //}

        if (scale.x != 1.0f)
        {
            this.matrix.a *= this.scale.x;
            this.matrix.b *= this.scale.x;
        }

        if (scale.y != 1.0f)
        {
            this.matrix.c *= this.scale.y;
            this.matrix.d *= this.scale.y;
        }

        this.matrix.tx = this.pos.x;
        this.matrix.ty = this.pos.y;
        matrix.CreateBox(scale.x,scale.y,-rotation,pos.x,pos.y);
        if (this.parent != null)
        {
            this.matrix.Concat(this.parent.matrix);
            startMatrix.CreateBox(this.startScale.x, this.startScale.y, -this.startRotation, this.startPos.x, this.startPos.y);
            startMatrix.Concat(this.parent.startMatrix);
            //deltaMatrix.SetTo(matrix.a-startMatrix.a,matrix.b-startMatrix.b,matrix.c-startMatrix.c,matrix.d-startMatrix.d,matrix.tx-startMatrix.tx,matrix.ty-startMatrix.ty);
        }

        //foreach (UgTrans bone in children)
        //{
        //    Debug.Log(bone.name + " --- " + "updateMatrix");
        //}

        foreach (UgTrans bone in this.children)
        {
            bone.updateMatrix();
        }

    }

    public void AddChild(UgTrans trans)
    {
        trans.parent = this;
        children.Add(trans);
    }


    protected GameObject testObj;
    public virtual GameObject CreateTest(Transform trans)
    {
        GameObject o = new GameObject(name);
        o.transform.SetParent(trans);
        o.transform.localScale = new Vector3(scale.x, scale.y, 1);
        o.transform.localPosition = pos;
        o.transform.localEulerAngles = new Vector3(0, 0, rotation);

        for (int i = 0; i < children.Count; i++)
        {
            children[i].CreateTest(o.transform);
        }
        testObj = o;
        return o;
    }
}
