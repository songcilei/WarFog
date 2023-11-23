using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public enum SampleDown
{
    None,
    down2Sample,
    down4Sample,
    down8Sample,
    down16Sample,
    down32Sample,
    down64Sample
}

public class ScreenToWorld : MonoBehaviour
{
    private Camera cam;
    
    public GameObject obj;
    public GameObject FogMgr;
    private Testing testing;
    public SampleDown SampleDown = (int)SampleDown.None;
    private RenderTexture _rt1;

    private DrawWarFogTex _drawWarfog;
    
    [ReadOnly]
    public int rtWidth;
    [ReadOnly]
    public int rtHeight;
    [ReadOnly]
    public Texture2D _tex2D;
    void Start()
    {
        cam = Camera.main;
        if (FogMgr==null)
        {
            FogMgr = GameObject.Find("FogMgr");
            testing = FogMgr.GetComponent<Testing>();
        }

        int sampleDownValue = GetSampleDownValue(SampleDown);
        rtWidth = Screen.width/sampleDownValue;
        rtHeight = Screen.height/sampleDownValue;
        _tex2D = new Texture2D(rtWidth, rtHeight);

        _drawWarfog = this.GetComponent<DrawWarFogTex>();
        Debug.Log("rtWidth:"+rtWidth);
        Debug.Log("rtHeight:"+rtHeight);

    }

    
    


    void Update()
    {
//这段考虑整个转换成协程 用于控制迭代次数  拯救性能傻瓜

        RaycastHit[] InfoRay = ProjectRayNDCCorners();

        Vector3 v_rightTop = InfoRay[0].point;
        Vector3 v_rightDown = InfoRay[1].point;
        Vector3 v_leftTop = InfoRay[2].point;
        Vector3 v_leftDown = InfoRay[3].point;

        
        
//---------------------------------------------------Input Not Fog
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos;
            Vector2 curentUV = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            
            testing.grid.SetFogStateScreenPos(curentUV, v_leftTop, v_leftDown, v_rightTop, v_rightDown,FogState.NotFog,out worldPos);
            
            Debug.DrawLine(cam.transform.position,worldPos,Color.blue,100f);
 
        }
//----------------------------------------------------Input Half Fog
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPos;
            Vector2 curentUV = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            
            testing.grid.SetFogStateScreenPos(curentUV, v_leftTop, v_leftDown, v_rightTop, v_rightDown,FogState.HalfFog,out worldPos);
            
            Debug.DrawLine(cam.transform.position,worldPos,Color.blue,100f);
 
        }
        
        
//---------------------------------------------------RT

//For Each Per Pixel
        for (int i = 0; i < rtWidth; i++)
        {
            for (int j = 0; j < rtHeight; j++)
            {
                
                Vector3 worldPos;
                Vector2 currenUV = new Vector2((float)i / rtWidth,(float)j/rtHeight);
                // Debug.Log(currenUV);
                FogState _fogState =
                testing.grid.GetFogStateForScreenPos(currenUV, v_leftTop, v_leftDown, v_rightTop, v_rightDown,out worldPos);


                switch (_fogState)
                {
                    case FogState.IsFog:
                        _tex2D.SetPixel(i,j,Color.black);
                        break;
                    case FogState.NotFog:
                        _tex2D.SetPixel(i,j,Color.white);
                        break;
                    case FogState.HalfFog:
                        _tex2D.SetPixel(i,j,Color.gray);
                        break;
                }
            }
        }
        _tex2D.Apply();
        
//==========================>draw warFog
        _drawWarfog._tex2D = _tex2D;
        _drawWarfog.mUpdate();
        //Shader.SetGlobalTexture("_WarFogMap",_tex2D);
    }
    
    RaycastHit[] ProjectRayNDCCorners()
    {
        RaycastHit[] InfoRay = new RaycastHit[4];
 


        Vector3 cor_rightTop = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector3 cor_rightDown = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector3 cor_leftTop = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 cor_leftDown = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        

        Ray ray1 = new Ray(cor_rightTop, cam.transform.forward);
        Ray ray2 = new Ray(cor_rightDown, cam.transform.forward);
        Ray ray3 = new Ray(cor_leftTop, cam.transform.forward);
        Ray ray4 = new Ray(cor_leftDown, cam.transform.forward);
        
        Physics.Raycast(ray1, out InfoRay[0]);
        Physics.Raycast(ray2, out InfoRay[1]);
        Physics.Raycast(ray3, out InfoRay[2]);
        Physics.Raycast(ray4, out InfoRay[3]);
        return InfoRay;
    }

    int GetSampleDownValue(SampleDown _sample)
    {
        switch (_sample)
        {
            case SampleDown.None:
                return 1;
            case SampleDown.down2Sample:
                return 2;
            case SampleDown.down4Sample:
                return 4;
            case SampleDown.down8Sample:
                return 8;
            case SampleDown.down16Sample:
                return 16;
            case SampleDown.down32Sample:
                return 32;
            case SampleDown.down64Sample:
                return 64;
            default:
                return 4;
        }
    }


}