using System.Collections.Generic;
using UnityEngine;

public class LevelDataHolder : MonoBehaviour
{
    public GridSystem gridSystem;
    public LevelDataSO levelDataSo;
    public bool[,,] pieceActivityArray;

    // Start is called before the first frame update
    void Start()
    {
        var localScale = levelDataSo.levelAreaScale;
        float lenght = localScale.z;
        float width = localScale.x;
        
        int lenghtCount = (int)(lenght / levelDataSo.pieceScale.z);
        int widthCount = (int)(width / levelDataSo.pieceScale.x);
        int heightCount = levelDataSo.layerCount;
        pieceActivityArray = ConvertArrayToMultiDimensionalArray(levelDataSo.isOpenArray,widthCount,heightCount,lenghtCount);
        
        GridCreate(levelDataSo);

    }

    public void GridCreate( LevelDataSO levelDataSO)
    {
        var localScale = levelDataSO.levelAreaScale;
        float lenght = localScale.z;
        float width = localScale.x;
        
        int lenghtCount = (int)(lenght / levelDataSO.pieceScale.z);
        int widthCount = (int)(width / levelDataSO.pieceScale.x);
        int heightCount = levelDataSO.layerCount;
    
        Vector3 startPos = new Vector3(-localScale.x / 2, 0, -localScale.z/2);
        
        // Debug.Log(widthCount);
        // Debug.Log(lenghtCount);
        // Debug.Log(heightCount);
        // Debug.Log(startPos);
        // Debug.Log(levelDataSO.pieceScale);
        // Debug.Log(levelDataSO.defEmptyPercent);
        // Debug.Log(levelDataSO.pieceKindCount);
        // Debug.Log(levelDataSO.pieceProperties.Count);


        gridSystem = new GridSystem
        (widthCount,
            heightCount,
            lenghtCount,
            transform.GetChild(0),
            startPos,
            levelDataSO.pieceScale,
            levelDataSO.defEmptyPercent,
            levelDataSO.pieceKindCount,
            levelDataSO.pieceProperties,
            levelDataSO.neededMatchCount,
            pieceActivityArray);
        LevelManager.Instance.currentGridSystem = gridSystem;
    }

    // public List<int> LoadFromJson()
    // {
    //
    //     var data = System.IO.File.ReadAllText("Assets/Resources/LevelSaveData/XKralTR.json");
    //     readableData.readableData = JsonUtility.FromJson<List<int>>(data);
    //     // string filePath = $"Assets/Resources/LevelSaveData/XKralTR.json";
    //     // string piecesActivityData = System.IO.File.ReadAllText(filePath);
    //     //
    //     // List<int> array = JsonUtility.FromJson<List<int>>(piecesActivityData);
    //     
    //     if(readableData.readableData.Count == 0)
    //     {
    //         print("null");
    //         return null;
    //     }
    //     foreach (var asd in readableData.readableData)
    //     {
    //          print(asd);
    //     }
    //     
    //     return readableData.readableData;
    // }

    public bool[,,] ConvertArrayToMultiDimensionalArray(List<int> array,int x, int y, int z)
    {
        bool[,,] piecesActivityArray = new bool[x, y, z];

        int index = 0;
        //if (array.Count == 0) return null;
       // Debug.Log(array[0]);
        for (int i = 0; i < piecesActivityArray.GetLength(1); i++)
        {
            for (int j = 0; j < piecesActivityArray.GetLength(0); j++)
            {
                for (int k = 0; k < piecesActivityArray.GetLength(2); k++)
                {
                    if (array[index] == 1)
                    {
                        piecesActivityArray[j, i, k] = true;
                    }
                    else
                    {
                        piecesActivityArray[j, i, k] = false;
                    } 
                    index++;
                }
            }
        }

        return piecesActivityArray;
    }
}

