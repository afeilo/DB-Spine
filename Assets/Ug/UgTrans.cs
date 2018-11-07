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



    public UgTrans parent = null;
    public List<UgTrans> children = new List<UgTrans>();

    public virtual void Init(UgData.TransformData data)
    {
        this.name = data.name;
        this.parentName = data.parent;
        this.pos = data.pos;
        this.scale = data.scale;
        this.rotation = data.rotation;
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
