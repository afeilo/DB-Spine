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
        //Play("stand");
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
                UgSprite d_data = slot.sprites[0];
                //foreach (UgSprite d_data in slot.sprites)
                //{

                    if (d_data.offset >= 0)
                    {
                        float targetWidth = 0;
                        float targetHeight = 0;
                        float targetX = 0;
                        float targetY = 0;
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
                            Vector3 vertex = new Vector3();
                            Vector2 uv = new Vector2();
                            vertex.x = vertexDatas.vertices[(d_data.offset + j) * 2];
                            vertex.y = vertexDatas.vertices[(d_data.offset + j) * 2 + 1];
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
                            UgBone bone = slot.parent;
                            Vector2 pos = d_data.pos;
                            while (bone != null) {
                                pos = new Vector2(pos.x * bone.scale.x, pos.y * bone.scale.y);
                                var r = 2*Mathf.PI*bone.rotation/360;
                                pos = new Vector2(pos.x * Mathf.Cos(r) + pos.y * Mathf.Sin(r), -pos.x * Mathf.Sin(r) + pos.y * Mathf.Cos(r));
                                pos = pos + bone.pos;
                                bone = bone.parent;
                            }
                            vertex.x = pos.x + x;
                            vertex.y = pos.y + y;
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
