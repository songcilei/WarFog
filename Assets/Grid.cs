using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public enum FogState{
    IsFog,
    NotFog,
    HalfFog
}

public class Grid : MonoBehaviour
{

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    
    
    private TextMesh[,] debugTextArray;
    //true => fog   false => not Fog
    private FogState[,] IsNotFog; 
    
    public Grid(int width,int height,float cellSize,Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        this.originPosition = originPosition;
        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];
        IsNotFog = new FogState[width, height];
        Debug.Log("Init");
        for (int x=0;x<gridArray.GetLength(0);x++) {
            for (int y=0;y<gridArray.GetLength(1);y++)
            {
                // Debug.Log(GetWorldPosition(x,y));
                IsNotFog[x, y] = FogState.IsFog;

                debugTextArray[x,y] = UtilsClass.CreateWorldText(IsNotFog[x, y].ToString(), null, GetWorldPosition(x, y)+new Vector3(cellSize,0,cellSize)*0.5f, 30, Color.white,
                    TextAnchor.MiddleCenter);
//DOTO:  这里有点问题  不知道为啥刷新不了图片                
                // Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x,y+1),Color.white,100f);
                // Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x+1,y),Color.white,100f);

            }
        }
        
        Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
        Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);

        // Debug.Log("2222222");
        SetValue(2,1,56);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public FogState GetFogState(int x,int y)
    {
        if (x>= 0 && y>= 0 && x<width && y<height)
        {
            return IsNotFog[x,y];
        }
        else
        {
            return FogState.IsFog;
        }

    }

    
    
    /// <summary>
    ///  ScreenPos =>vector3(0->screen.width,0->height,0)=>input.mousePosition
    /// </summary>
    /// <param name="ScreenPos"></param>
    /// <param name="leftTop"></param>
    /// <param name="leftDown"></param>
    /// <param name="rightTop"></param>
    /// <param name="rightDown"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public FogState GetFogStateForScreenPos(Vector2 ScreenPos,Vector3 leftTop,Vector3 leftDown,Vector3 rightTop,Vector3 rightDown,out Vector3 worldPos)
    {
        Vector3 screenPos = new Vector3(ScreenPos.x,0,ScreenPos.y);
        Vector3 worldTop = Vector3.Lerp(leftTop, rightTop, screenPos.x);
        Vector3 worldDown = Vector3.Lerp(leftDown, rightDown, screenPos.x);
        worldPos = Vector3.Lerp(worldTop, worldDown, 1-screenPos.z);
        
        
        int x, y;
        GetXZ(worldPos,out x,out y);
        return GetFogState(x, y);
    }
    /// <summary>
    /// ScreenPos =>vector3(0->screen.width,0->height,0)=>input.mousePosition
    /// </summary>
    /// <param name="ScreenPos"></param>
    /// <param name="leftTop"></param>
    /// <param name="leftDown"></param>
    /// <param name="rightTop"></param>
    /// <param name="rightDown"></param>
    /// <param name="worldPos"></param>
    public void SetFogStateScreenPos(Vector2 ScreenPos,Vector3 leftTop,Vector3 leftDown,Vector3 rightTop,Vector3 rightDown,FogState fotState,out Vector3 worldPos)
    {
        Vector3 screenPos = new Vector3(ScreenPos.x,0,ScreenPos.y);
        Vector3 worldTop = Vector3.Lerp(leftTop, rightTop, screenPos.x);
        Vector3 worldDown = Vector3.Lerp(leftDown, rightDown, screenPos.x);
        worldPos = Vector3.Lerp(worldTop, worldDown, 1-screenPos.z);
        
        
        int x, y;
        GetXZ(worldPos,out x,out y);
        SetFog(x,y,fotState);
    }


    public void SetFog(int x,int y,FogState value)
    {
        if (x>=0 && y >=0 && x<width && y < height)
        {
            IsNotFog[x, y] = value;
            debugTextArray[x, y].text = IsNotFog[x, y].ToString();
        }
    }







    private Vector3 GetWorldPosition(int x,int z)
    {
        return new Vector3(x, 0,z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition,out int x,out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetValue(int x,int y,int value)
    {
        if (x>=0 && y >=0 && x<width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }

    }


    public void SetValue(Vector3 worldPosition,int value)
    {
        int x, z;
        GetXZ(worldPosition,out x,out z);
        SetValue(x,z,value);
    }

    public int GetValue(int x,int y)
    {
        if (x>= 0 && y>= 0 && x<width && y<height)
        {
            return gridArray[x, y];
        }
        else
        {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXZ(worldPosition,out x,out y);
        return GetValue(x, y);
    }

}