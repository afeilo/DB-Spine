﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgRole : MonoBehaviour
{
    public UgData data;
    public Dictionary<string, UgArmature> armatures = new Dictionary<string, UgArmature>();
    private Mesh sharedMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRender;
    UgArmature currentArmature;

    void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRender = gameObject.AddComponent<MeshRenderer>();

        meshRender.material = data.matrial;
    }
    void Start()
    {
        Create();
    }


    void Create()
    {
        if (data == null)
            return;

        for (int i = 0; i < data.armatures.Length; i++)
        {
            UgArmature arm = new UgArmature();
            armatures.Add(data.armatures[i].name, arm);
            arm.Init(data.armatures[i]);
            currentArmature = arm;
            arm.CreateTest(transform);
        }
        Play("attack");
    }

    public void SetCurrentAramture(string name)
    {
        currentArmature = armatures[name];
    }


    bool isPlaying = false;
    float pt;

    public void Play(string clipName)
    {
        pt = Time.time;
        isPlaying = true;
        currentArmature.Play(clipName);
    }

    void Update()
    {
        if (isPlaying)
        {
            float delta = Time.time - pt;
            pt = Time.time;
            currentArmature.UpdateFrame(delta);
        }
    }

    void LateUpdate()
    {
        sharedMesh = new Mesh();
        int k = 0;
        var displayData = data.armatures[0].displayDatas;
        var vertexDatas = data.armatures[0].vertexDatas;
        //Vector3[] vertices = new Vector3[count];
        //Vector2[] uvs = new Vector2[count];
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        var textureDatas = data.armatures[0].textureDatas;
        var sourceWidth = textureDatas.width;
        var sourceHeight = textureDatas.height;
        var subTextureDatas = textureDatas.datas;

        //foreach (UgBone bone in currentArmature.bonesList)
        //{
        foreach (UgSlot slot in currentArmature.slotList)
            {
                UgSprite d_data = slot.children[0] as UgSprite;
                //foreach (UgSprite d_data in slot.sprites)
                //{

                    if (d_data.offset >= 0)
                    {
                        float targetWidth = 0;
                        float targetHeight = 0;
                        float targetX = 0;
                        float targetY = 0;
                        int weightoffset = d_data.weightOffset;
                        for (int j = 0; j < subTextureDatas.Length; j++)
                        {
                            if (subTextureDatas[j].name == d_data.name)
                            {
                                targetWidth = subTextureDatas[j].width;
                                targetHeight = subTextureDatas[j].height;
                                targetX = subTextureDatas[j].x;
                                targetY = subTextureDatas[j].y;
                                break;
                            }
                        }
                        var count = vertices.Count;
                        for (int j = 0; j < d_data.count; j++)
                        {
                            Vector2 vertex = Vector2.zero;
                            Vector2 uv = new Vector2();

                            
                            Vector2 pos = new Vector2(vertexDatas.vertices[(d_data.offset + j) * 2],vertexDatas.vertices[(d_data.offset + j) * 2 + 1]);
                            Vector2 result = Vector2.zero;
                            int boneCount = (int)vertexDatas.weights[weightoffset++];
                            UgMatrix2D matrix = new UgMatrix2D();
                            Vector2 scale = Vector2.one;
                            float rotation = 0f;
                            Vector2 trans = Vector3.zero;
                            for (int c = 0; c < boneCount; c++)
                            {
                                UgBone bone = currentArmature.bonesList[(int)vertexDatas.weights[weightoffset]];
                                weightoffset++;
                                if (bone != null)
                                {
                                    //Debug.Log(bone.deltaMatrix.ToString());
                                    //Debug.Log(bone.matrix.tx - bone.startMatrix.tx);
                                    //matrix.a +=  (bone.matrix.a - bone.startMatrix.a) * vertexDatas.weights[weightoffset];
                                    //matrix.b +=  (bone.matrix.b - bone.startMatrix.b) * vertexDatas.weights[weightoffset];
                                    //matrix.c +=  (bone.matrix.c - bone.startMatrix.c) * vertexDatas.weights[weightoffset];
                                    //matrix.d +=  (bone.matrix.d - bone.startMatrix.d) * vertexDatas.weights[weightoffset];
                                    //matrix.tx += (bone.matrix.tx - bone.startMatrix.tx) * vertexDatas.weights[weightoffset];
                                    //matrix.ty += (bone.matrix.ty - bone.startMatrix.ty) * vertexDatas.weights[weightoffset];
                                    //matrix.a += (bone.deltaMatrix.a) * vertexDatas.weights[weightoffset];
                                    //matrix.b += (bone.deltaMatrix.b) * vertexDatas.weights[weightoffset];
                                    //matrix.c += (bone.deltaMatrix.c) * vertexDatas.weights[weightoffset];
                                    //matrix.d += (bone.deltaMatrix.d) * vertexDatas.weights[weightoffset];
                                    //matrix.tx += (bone.deltaMatrix.tx) * vertexDatas.weights[weightoffset];
                                    //matrix.ty += (bone.deltaMatrix.ty) * vertexDatas.weights[weightoffset];
                                    //scale += (bone.scale - bone.startScale) * vertexDatas.weights[weightoffset];
                                    //rotation += (bone.rotation - bone.startRotation) * vertexDatas.weights[weightoffset];
                                    //trans += (bone.pos - bone.startPos) * vertexDatas.weights[weightoffset];
                                    scale += new Vector2(bone.matrix.GetScaleX()-bone.startMatrix.GetScaleX(),bone.matrix.GetScaleY()-bone.startMatrix.GetScaleY())*vertexDatas.weights[weightoffset];
                                    rotation += (bone.matrix.GetAngle() - bone.startMatrix.GetAngle()) * vertexDatas.weights[weightoffset];
                                    trans += new Vector2(bone.matrix.tx - bone.startMatrix.tx,bone.matrix.ty-bone.startMatrix.ty) * vertexDatas.weights[weightoffset];
                                    //vertex += bone.deltaMatrix.TransformPoint(pos.x, pos.y) * vertexDatas.weights[weightoffset];
                                }
                                weightoffset++;
                            }
                            matrix.CreateBox(scale.x, scale.y, rotation, trans.x, trans.y);
                            //matrix.Concat(d_data.parent.matrix);
                            vertex = matrix.TransformPoint(pos.x, pos.y);
                            //pos = new Vector2(pos.x * scale.x, pos.y * scale.y);
                            //var r = -2 * Mathf.PI * rotation / 360;
                            //pos = new Vector2(pos.x * Mathf.Cos(r) - pos.y * Mathf.Sin(r), pos.x * Mathf.Sin(r) + pos.y * Mathf.Cos(r));
                            //pos = pos + trans;
                            //vertex = pos;
                            //Vector2 scale = Vector2.one;
                            //float rotation = 0f;
                            //Vector2 trans = Vector3.zero;
                            //for (int c = 0; c < boneCount; c++)
                            //{
                            //    UgBone bone = currentArmature.bonesList[(int)vertexDatas.weights[weightoffset]];
                            //    weightoffset++;
                            //    if (bone != null)
                            //    {
                            //        scale += (bone.scale-bone.startScale) * vertexDatas.weights[weightoffset];
                            //        rotation += (bone.rotation-bone.startRotation) * vertexDatas.weights[weightoffset];
                            //        trans += (bone.pos-bone.startPos) * vertexDatas.weights[weightoffset];
                            //    }
                            //    weightoffset++;
                            //}
                            //pos = new Vector2(pos.x * scale.x, pos.y * scale.y);
                            //var r = 2 * Mathf.PI * rotation / 360;
                            //pos = new Vector2(pos.x * Mathf.Cos(r) - pos.y * Mathf.Sin(r), pos.x * Mathf.Sin(r) + pos.y * Mathf.Cos(r));
                            //pos = pos + trans;
                            //vertex = pos;
                            uv.x = (vertexDatas.uvs[(d_data.offset + j) * 2] * targetWidth + targetX) / sourceWidth;
                            uv.y = 1 - (vertexDatas.uvs[(d_data.offset + j) * 2 + 1] * targetHeight + targetY) / sourceHeight;
                            //vertices[d_data.offset + j] = vertex;
                            //uvs[d_data.offset + j] = uv;
                            vertices.Add(vertex);
                            uvs.Add(uv);
                        }

                        for (int j = 0; j < d_data.triangleCount; )
                        {
                            triangles.Add(count + vertexDatas.triangles[d_data.triangleOffset + j++]);
                            triangles.Add(count + vertexDatas.triangles[d_data.triangleOffset + j++]);
                            triangles.Add(count + vertexDatas.triangles[d_data.triangleOffset + j++]);
                        }

                    }
                    //图片
                    else
                    {
                        if (k > 5) continue;
                        k++;
                        float targetWidth = 0;
                        float targetHeight = 0;
                        float targetX = 0;
                        float targetY = 0;
                        float pivotX = 0;
                        float pivotY = 0;
                        for (int j = 0; j < subTextureDatas.Length; j++)
                        {
                            if (subTextureDatas[j].name == d_data.name)
                            {
                                targetWidth = subTextureDatas[j].width;
                                targetHeight = subTextureDatas[j].height;
                                targetX = subTextureDatas[j].x;
                                targetY = subTextureDatas[j].y;
                                break;
                            }
                        }
                        pivotX = d_data.pivot.x;
                        pivotY = d_data.pivot.y;
                        var count = vertices.Count;
                        float x = 0, y = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            int u = 0, v = 0;
                            if (0 == j)
                            {
                                u = 0;
                                v = 0;
                                x = -targetWidth * pivotX;
                                y = -targetHeight * pivotY;
                            }
                            else if (1 == j)
                            {
                                u = 1;
                                v = 0;
                                x = targetWidth * (1 - pivotX);
                                y = -targetHeight * pivotY;

                            }
                            else if (2 == j)
                            {
                                u = 1;
                                v = 1;
                                x = targetWidth * (1 - pivotX);
                                y = targetHeight * (1 - pivotY);
                            }
                            else if (3 == j)
                            {
                                u = 0;
                                v = 1;
                                x = -targetWidth * pivotX;
                                y = targetHeight * (1 - pivotY);
                            }

                            Vector3 vertex = new Vector3();
                            Vector2 uv = new Vector2();
                            Vector2 pos = d_data.matrix.TransformPoint(x, y);
                            //Vector2 pos = new Vector2(x * (d_data.scale).x, y * (d_data.scale).y);
                            //var r = -2 * Mathf.PI * (d_data.rotation) / 360;
                            //pos = new Vector2(pos.x * Mathf.Cos(r) - pos.y * Mathf.Sin(r), pos.x * Mathf.Sin(r) + pos.y * Mathf.Cos(r));
                            //pos = pos + d_data.pos;
                            //var bone = d_data.parent;
                            //while (bone != null)
                            //{
                            //    pos = new Vector2(pos.x * (bone.scale).x, pos.y * (bone.scale).y);
                            //    r = -2 * Mathf.PI * (bone.rotation) / 360;
                            //    pos = new Vector2(pos.x * Mathf.Cos(r) - pos.y * Mathf.Sin(r), pos.x * Mathf.Sin(r) + pos.y * Mathf.Cos(r));
                            //    pos = pos + (bone.pos);
                            //    bone = bone.parent;
                            //}
                            vertex.x = pos.x;
                            vertex.y = pos.y;
                            //Debug.Log(vertex.x + "," + vertex.y);
                            //Debug.Log("-----------");
                            uv.x = (u * targetWidth + targetX) / sourceWidth;
                            uv.y = 1 - (v * targetHeight + targetY) / sourceHeight;
                            //vertices[d_data.offset + j] = vertex;
                            //uvs[d_data.offset + j] = uv;
                            vertices.Add(vertex);
                            uvs.Add(uv);

                        }

                        triangles.Add(count + 0);
                        triangles.Add(count + 3);
                        triangles.Add(count + 2);
                        triangles.Add(count + 2);
                        triangles.Add(count + 1);
                        triangles.Add(count + 0);
                    //}
                    //continue;
                }
            }
        //}
        sharedMesh.vertices = vertices.ToArray();
        sharedMesh.uv = uvs.ToArray();

        sharedMesh.triangles = triangles.ToArray();
        meshFilter.sharedMesh = sharedMesh;

    }
}
