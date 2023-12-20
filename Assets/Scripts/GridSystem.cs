using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridSystem
{
    public int width;
    public int height;
    public int lenght;
    public Vector3 startPos;
    public int pieceKindCount;
    public List<PieceProperties> pieceProperties;
    public List<Piece> allPieces = new List<Piece>();

    public Vector3 pieceSize;
    public Piece[,,] gridArray;
    public bool[,,] isOpenArray;

    public int neededMatchCount;
    //public int startPieceCount;

    public GridSystem(int width,int height,int lenght,Transform layerParent ,Vector3 startPos,Vector3 pieceSize,
        float defEmptyPercent,int pieceKindCount,List<PieceProperties> pieceProperties,int neededMatchCount,bool[,,] isOpenArray = null)
    {
        this.width = width;
        this.height = height;
        this.lenght = lenght;
        this.startPos = startPos;
        this.pieceSize = pieceSize;
        this.pieceKindCount = pieceKindCount;
        this.pieceProperties = pieceProperties;
        this.neededMatchCount = neededMatchCount;
        this.isOpenArray = isOpenArray;
        
        gridArray = new Piece[width, height, lenght];

        for (int y = 0; y < gridArray.GetLength(1); y++)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    Vector3 piecePos = new Vector3(
                        startPos.x + x * pieceSize.x, 
                        startPos.y + y * pieceSize.y, 
                        startPos.z + z * pieceSize.z);
                    
                    if(y % 2 == 1)
                        piecePos += new Vector3(pieceSize.x / 2 , 0, pieceSize.z / 2 );

                    GameObject pieceObj = GameControl.Instance.CreatePieceObject(GameControl.Instance.piecePrefab,piecePos,
                        Quaternion.identity,layerParent.GetChild(y).transform);
                    Piece pieceComp = pieceObj.GetComponent<Piece>();

                    
                    pieceComp.arrayPos = new Vector3Int(x, y, z);
                    gridArray[x, y, z] = pieceComp;
                    allPieces.Add(pieceComp);
                    
                    Vector3 pieceScale = pieceSize - (pieceSize * defEmptyPercent) / 100;
                    pieceComp.canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(pieceScale.x, pieceScale.z);
                    pieceComp.unavailableImage.GetComponent<RectTransform>().sizeDelta = new Vector2(pieceScale.x, pieceScale.z);
                    pieceComp.mesh.transform.localScale = pieceScale;
                    //pieceComp.pos = new int [iii,i,ii];

                    // piece.transform.position = localPos;
                }
            }
        }

        if (isOpenArray != null)
        {
            SetPiecesActivity();
        }
        else
        {
            SetPiecesType();
            SetPiecesNeighbours();
        }

    }
    
    public void SetPiecesType()
    {
        List<Piece> activePieces = new List<Piece>();

        foreach (var piece in allPieces)
        {
            if (piece.isOpen)
            {
                activePieces.Add(piece);
            }
        }
        
        int pieceCount = activePieces.Count;
        int totalMatchCount = pieceCount / neededMatchCount;
        int typeOrder = 0;

        for (int i = 0; i < totalMatchCount; i++)
        {
            for (int j = 0; j < pieceCount/totalMatchCount; j++)
            {
               
                Piece piece = activePieces[Random.Range(0, activePieces.Count)];

                int type = typeOrder % pieceKindCount;
                
                piece.mainImage.sprite = pieceProperties[type].image;
                piece.type = type;
                activePieces.Remove(piece);
            }

            typeOrder++;
        }
    }

    public void SetPiecesActivity()
    {
        int startPieceCount = 0;
        for (int y = 0; y < gridArray.GetLength(1); y++)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    if (isOpenArray[x, y, z])
                    {
                        gridArray[x, y, z].isOpen = true;
                        startPieceCount++;
                    }
                    else
                    {
                        gridArray[x, y, z].isOpen = false;
                        allPieces.Remove(gridArray[x, y, z]);
                    }
                    gridArray[x,y,z].SetAvailable();
                }
            }
        }

        LevelManager.Instance.startPieceCount = startPieceCount;
        SetPiecesType();
        SetPiecesNeighbours();
    }

    public void SetPiecesNeighbours()
    {
        foreach (var piece in allPieces)
        {
            for (int x = piece.arrayPos.x-1; x < piece.arrayPos.x + 1; x++)
            {
                for (int z = piece.arrayPos.z-1; z < piece.arrayPos.z + 1; z++)
                {
                    if(x >= 0 && x < gridArray.GetLength(0) && z >= 0 && z < gridArray.GetLength(2))
                    {
                        if (piece.arrayPos.y + 1 < gridArray.GetLength(1))
                        {
                            piece.upNeighbours.Add( gridArray[x,piece.arrayPos.y + 1,z]);
                        }
                    }
                }
            }
            
            
            for (int x = piece.arrayPos.x; x < piece.arrayPos.x + 2; x++)
            {
                for (int z = piece.arrayPos.z; z < piece.arrayPos.z + 2; z++)
                {
                    if(x >= 0 && x < gridArray.GetLength(0) && z >= 0 && z < gridArray.GetLength(2))
                    {
                        if (piece.arrayPos.y - 1 >= 0)
                        {
                            piece.downNeighbours.Add( gridArray[x,piece.arrayPos.y - 1,z]);
                        }
                    }
                }
            }
        }
    }
    
}
