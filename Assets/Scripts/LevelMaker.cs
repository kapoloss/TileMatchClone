using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelMaker : Singleton<LevelMaker>
{
    public LevelValuesSO levelValuesSO;

    public GameObject levelParent;
    public LevelDataHolder levelDataHolder;
    public GameObject gameAreaBorder;
    public Transform layerParent;
    public GameObject prefab;
    public GridSystem gridSystem;

    [HideInInspector]public List<Piece> allPieces;

    public enum EditMode
    {
        Yes,
        No,
    }

    [Title("Edit Mode")]
    [EnumToggleButtons]
    public EditMode editMode;

    [HideInInspector] public List<GameObject> unEnablePieces;

    private void Awake()
    {
        
        if (levelParent != null && Application.isPlaying)
        {
            Destroy(levelParent);
            gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if(Selection.gameObjects.Length > 0  && editMode == EditMode.Yes)
        {
            GameObject[] selectedPieces = Selection.gameObjects;

            for (int i = 0; i < selectedPieces.Length; i++)
            {
                GameObject piece = selectedPieces[i];

                if (piece.layer == 3)
                {
                    if(piece.GetComponent<Piece>() == null)
                    {
                        if(piece.transform.parent.TryGetComponent<Piece>(out Piece piece1))
                        {
                            piece = piece1.gameObject;
                        }
                        else
                        {
                            if(piece.transform.parent.parent.TryGetComponent<Piece>(out Piece piece2))
                            {
                                piece = piece2.gameObject;
                            }
                        }
                    }
                    Piece pieceComp = piece.GetComponent<Piece>();
                    
                    pieceComp.mesh.SetActive(false);
                    pieceComp.canvas.SetActive(false);
                    pieceComp.isOpen = false;
                    

                    if(!unEnablePieces.Contains(piece))
                        unEnablePieces.Add(piece);
                }


            }

            if(Selection.activeGameObject.layer == 3)
            {
                Selection.activeGameObject = this.gameObject;
            }
           
            
        }
    }

    [Button]
    public void Undo()
    {
        if (unEnablePieces.Count > 0)
        {
            GameObject undoPiece = unEnablePieces[^1];
            Piece undoPieceComp = undoPiece.GetComponent<Piece>();
            
            undoPieceComp.mesh.SetActive(true);
            undoPieceComp.canvas.SetActive(true);
            
            undoPiece.GetComponent<Piece>().isOpen = false;
            unEnablePieces.Remove(undoPiece);
        }
       
    }

    [Button]
    public void ClearScene()
    {
        allPieces.Clear();

        foreach (Transform layer in layerParent.transform)
        {
            int totalChild = layer.transform.childCount;

            for (int i = 0; i < totalChild; i++)
            {
                DestroyImmediate(layer.transform.GetChild(0).gameObject);
            }
        }

        unEnablePieces.Clear();
    }

    [Button]
    public void SeePieces()
    {
        ClearScene();

        var localScale = gameAreaBorder.transform.localScale;
        float lenght = localScale.z;
        float width = localScale.x;
        
        int lenghtCount = (int)(lenght / levelValuesSO.pieceScale.z);
        int widthCount = (int)(width / levelValuesSO.pieceScale.x);
        int heightCount = levelValuesSO.layerCount;
        
        Vector3 startPos = new Vector3(-localScale.x / 2, 0, -localScale.z/2);
        
        
        gridSystem = new GridSystem(
            widthCount,
            heightCount,
            lenghtCount,
            layerParent,
            startPos,
            levelValuesSO.pieceScale,
            levelValuesSO.defEmptyPercent,
            levelValuesSO.pieceKindCount,
            levelValuesSO.pieceProperties,
            levelValuesSO.neededPieceForMatch);
    }

    [Button]
    public void SaveLevel()
    {
        int activePieceCount = ActivePieceCount();
        if (activePieceCount % gridSystem.neededMatchCount != 0)
        {
            
            Debug.LogError("totalPiece count cant devided matchNeedCount");
        }
        else
        {
            CreateLevelDataSO();
            string localPath = "Assets/Prefabs/Levels/" + "Level" + PrefabCount("Assets/Prefabs/Levels","Prefab") + ".prefab";

            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            PrefabUtility.SaveAsPrefabAssetAndConnect(levelParent, localPath, InteractionMode.UserAction);

            PrefabUtility.UnpackPrefabInstance(levelParent, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        
    }

    public int ActivePieceCount()
    {
        int i = 0;
        foreach (var piece in gridSystem.allPieces)
        {
            if (piece.isOpen)
            {
                i++;
            }
        }

        return i;
    }

    public int PrefabCount(string filePath,string fileType)
    {
        int prefabCount = 0;

        string[] guids = AssetDatabase.FindAssets($"t:{fileType}", new[] { filePath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (PrefabUtility.IsPartOfPrefabAsset(asset))
            {
                prefabCount++;
            }
        }

        return prefabCount;
    }
    
    public int SOCount(string filePath,string fileType)
    {
        int prefabCount = 0;

        string[] guids = AssetDatabase.FindAssets($"t:{fileType}", new[] { filePath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (FindObjectOfType(typeof(LevelDataSO)))
            {
                prefabCount++;
            }
        }

        return prefabCount;
    }

    public void CreateLevelDataSO()
    {
        LevelDataSO levelDataSo = ScriptableObject.CreateInstance<LevelDataSO>();
        levelDataSo.pieceProperties = levelValuesSO.pieceProperties;
        levelDataSo.layerCount = levelValuesSO.layerCount;
        levelDataSo.pieceScale = levelValuesSO.pieceScale;
        levelDataSo.defEmptyPercent = levelValuesSO.defEmptyPercent;
        levelDataSo.levelAreaScale = gameAreaBorder.transform.localScale;
        levelDataSo.pieceKindCount = levelValuesSO.pieceKindCount;
        levelDataSo.neededMatchCount = levelValuesSO.neededPieceForMatch;
        levelDataSo.isOpenArray = SetPiecesActiveInfo();
        SetPiecesActiveInfo();

        levelDataHolder.levelDataSo = levelDataSo;

        foreach (Transform layer in layerParent.transform)
        {
            int pieceCount = layer.transform.childCount;
            
            for (int i = 0; i < pieceCount; i++)
            {
                DestroyImmediate(layer.transform.GetChild(0).gameObject);
            }
        }
        //SaveToJson(piecesActivityArray,levelDataSo);
        
        

        string localPath = "Assets/Resources/ScriptableObjects/" + "LevelDataSO" + SOCount("Assets/Resources/ScriptableObjects","LevelDataSO") + ".asset";

        AssetDatabase.CreateAsset(levelDataSo, localPath);
        AssetDatabase.SaveAssets();

        Debug.Log("ScriptableObject created and saved at: " + localPath);
    }

    public List<int> SetPiecesActiveInfo()
    {
        List<int> boolArray = new List<int>();
        int order = 0;
        
        for (int y = 0; y < gridSystem.gridArray.GetLength(1); y++)
        {
            for (int x = 0; x < gridSystem.gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridSystem.gridArray.GetLength(2); z++)
                {
                    if (gridSystem.gridArray[x, y, z].isOpen)
                    {
                        boolArray.Add(1);
                    }
                    else
                    {
                        boolArray.Add(0);
                    }

                    order++;
                }
            }
        }

        return boolArray;
    }

    // public void SaveToJson(List<int> piecesActivityArray,LevelDataSO levelDataSO)
    // {
    //     var savingData = new SavingData()
    //     {
    //         piecesActivityData = SetPiecesActiveInfo()
    //     };
    //
    //     string piecesActivityData = JsonUtility.ToJson(savingData);
    //     string filePath = $"Assets/Resources/LevelSaveData/XKralTR.json";
    //     print(filePath);
    //     System.IO.File.WriteAllText(filePath,piecesActivityData);
    // }


}

// [System.Serializable]
// public class SavingData
// {
//     public List<int> piecesActivityData ;
// }
