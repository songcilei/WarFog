using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawWarFogTex : MonoBehaviour
{

    public float _BlurSize;
    public float LerpSpeed;
    public Material _mat;
    public Material _warFogMat;
    public string k_tag = "BoxBlur";
    private static readonly int BlurSizeID = Shader.PropertyToID("_BlurOffset");
    private static readonly int _rt = Shader.PropertyToID("_rt");
    private static readonly int _rtHalf = Shader.PropertyToID("_rtHalf");
    private static readonly int _rtQuat = Shader.PropertyToID("_rtQuat");
    private static readonly int _WarFogID = Shader.PropertyToID("_WarFogMap");
    private static readonly int _SourTex = Shader.PropertyToID("_SourTex");
    private static readonly int _LerpSpeed = Shader.PropertyToID("_LerpSpeed");
    private static readonly int _OldRt = Shader.PropertyToID("_OldMainTex");
    private CommandBuffer _cmd;
    public SampleDown _samepleDown = SampleDown.None;
    private int samDown = 0;
    public Texture2D _tex2D;
    public RenderTexture oldTex;
    void Start()
    {
        oldTex = RenderTexture.GetTemporary(Screen.width, Screen.height);

    }
    
    

    // Update is called once per frame
    public void mUpdate()
    {
        _cmd = new CommandBuffer();
        samDown = GetSampleDownValue(_samepleDown);
        _cmd.GetTemporaryRT(_rt,Screen.width/samDown,Screen.height/samDown,1,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);
        _cmd.GetTemporaryRT(_rtHalf,Screen.width/2/samDown,Screen.height/2/samDown,1,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);
        _cmd.GetTemporaryRT(_rtQuat,Screen.width/4/samDown,Screen.height/4/samDown,1,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);
        _cmd.GetTemporaryRT(_OldRt,Screen.width/samDown,Screen.height/samDown,1,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);

        _cmd.SetGlobalFloat(BlurSizeID,_BlurSize);
// Kawase =>1
        _cmd.Blit(_tex2D,_rt,_mat,0);
        
//Kawase =>2
        _cmd.Blit(_rt,_rtHalf,_mat,0);        
        
//Kawase =>3
        _cmd.Blit(_rtHalf,_rtQuat,_mat,0);

//Kawase =>4
        _cmd.Blit(_rtQuat,_rtHalf,_mat,0);
//Kawase =>5
        _cmd.Blit(_rtHalf,_rt,_mat,0);
        
// Fade RT
        _cmd.SetGlobalFloat(_LerpSpeed, LerpSpeed);
        _cmd.SetGlobalTexture(_SourTex,_rt);
        
//set Global Texture

        // _cmd.SetGlobalTexture(_WarFogID,_OldRt);
        Graphics.ExecuteCommandBuffer(_cmd);
        
        _cmd.Release();

    }


    public void Update()
    {
        Shader.SetGlobalTexture("_OldMainTex",oldTex);
        Graphics.Blit(null,oldTex,_mat,1);

        Shader.SetGlobalTexture("_WarFogMap",oldTex);
        // RenderTexture.ReleaseTemporary(oldTex);
    }


    public void SetTex2D(Texture2D _tex2D)
    {
        this._tex2D = _tex2D;
    }

    private void OnDestroy()
    {

        if (_cmd==null)
        {
            return;
        }

        if (_rt!=null)
        {
            _cmd.ReleaseTemporaryRT(_rt);
        }

        if (_rtHalf!=null)
        {
            _cmd.ReleaseTemporaryRT(_rtHalf);
        }

        if (_rtQuat!=null)
        {
            _cmd.ReleaseTemporaryRT(_rtQuat);

        }
        
        RenderTexture.ReleaseTemporary(oldTex);

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
            default:
                return 6;
        }
    }
    
}
