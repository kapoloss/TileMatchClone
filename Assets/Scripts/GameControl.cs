using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Lofelt.NiceVibrations;

public class GameControl : Singleton<GameControl>
{
    public GameObject piecePrefab;
    public LevelManager levelManager;
    public GameObject levelAreaBorder;
    public List<GameObject> layers;

    public GameObject takenPieceBoard;
    public int maxTakenPieceCount;
    public List<Piece> takenPieces;
    public int neededPieceForMatch;
    private static readonly Vector3 BoardPieceScale = new Vector3(0.25f,0.07f,0.25f);
    private Camera _camera;

    [SerializeField]public List<BaseSkillActionSO> skillActionSos;
    //public GameInputs gameInputs;

    //public InputAction click;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
        
    }

    private void Start()
    {
        _camera = Camera.main;
       // Debug.Log(gameInputs.ReadValue<Vector2>());
       skillActionSos[0].Skill();
       
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouch();
        }
    }


    public GameObject CreatePieceObject(GameObject obj, Vector3 tr, Quaternion rotation,Transform parent)
    {
        return Instantiate(obj, tr, rotation, parent);
    }

    private void CheckTouch()
    {
        Piece piece = FindTouchPiece(_camera!.ScreenToWorldPoint(Input.mousePosition));

        if (piece is not null)
        {
            if (!piece.isBlocked && takenPieces.Count < maxTakenPieceCount)
            {
                piece.isTaken = true;
                foreach (var downNeighbour in piece.downNeighbours)
                {
                    downNeighbour.SetAvailable();
                }
                TakePiece(piece);
            }
        }
    }

    private Piece FindTouchPiece(Vector3 mouseWorldPos)
    {
        mouseWorldPos -= levelManager.currentGridSystem.startPos;
       
        for (int y = levelManager.currentGridSystem.height - 1; y >= 0; y--)
        {
            Vector3 _mouseWorldPos = mouseWorldPos;

            var defPos = new Vector3(levelManager.currentGridSystem.pieceSize.x /2 , 0,
                levelManager.currentGridSystem.pieceSize.z/2 );

            if (y % 2 == 0)
            {
                _mouseWorldPos += defPos;
            }
            
            int x = Mathf.FloorToInt(_mouseWorldPos.x / levelManager.currentGridSystem.pieceSize.x);
            int z = Mathf.FloorToInt(_mouseWorldPos.z / levelManager.currentGridSystem.pieceSize.z);
            
            if (x >= 0 && z >= 0 && x < levelManager.currentGridSystem.width &&
                z < levelManager.currentGridSystem.lenght)
            {
                Piece piece = levelManager.currentGridSystem.gridArray[x, y, z];

                if (piece.isOpen && !piece.isTaken)
                {
                    return piece;
                }
                    
            }
        }

        return null;
    }

    public Vector3 FindPiecePosition(Piece piece)
    {
        Vector3 piecePos = new Vector3(
            levelManager.currentGridSystem.startPos.x + piece.arrayPos.x * levelManager.currentGridSystem.pieceSize.x, 
            levelManager.currentGridSystem.startPos.y + piece.arrayPos.y * levelManager.currentGridSystem.pieceSize.y, 
            levelManager.currentGridSystem.startPos.z + piece.arrayPos.z * levelManager.currentGridSystem.pieceSize.z);
                    
        if(piece.arrayPos.y % 2 == 1)
            piecePos += new Vector3(levelManager.currentGridSystem.pieceSize.x / 2 , 0, levelManager.currentGridSystem.pieceSize.z / 2 );

        return piecePos;
    }
    

    private void TakePiece(Piece piece)
    {
        int targetIndex = FindIndexAtBoard(piece);

        takenPieces.Insert(targetIndex,piece);
        
        for (int i = targetIndex + 1 ; i < takenPieces.Count; i++)
        {
            StartCoroutine(SetNewPos(takenPieces[i],2,false));
        }
        StartCoroutine(SetNewPos(piece,10,true));
        
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
    }

    private IEnumerator SetNewPos(Piece piece,float speed,bool takenPiece)
    {
        if (takenPiece)
        {
            transform.DOScale(BoardPieceScale, 0.2f);
        }
        while (Vector3.Distance(piece.transform.position, GetTargetPos(takenPieces.IndexOf(piece))) > 0.01f)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position,
                GetTargetPos(takenPieces.IndexOf(piece)), Time.deltaTime * speed);
            yield return null;
        }
        if(takenPiece)
           CheckMatch(piece);
    }

    private int FindIndexAtBoard(Piece piece)
    {
        for (int i = takenPieces.Count -1; i >= 0; i--)
        {
            if (takenPieces[i].type == piece.type)
            {
                return i + 1;
            }
        }

        return takenPieces.Count;
    }

    private Vector3 GetTargetPos(int index)
    {
        Vector3 boardPos = takenPieceBoard.transform.position;
        Vector3 boardScale = takenPieceBoard.transform.localScale;

        float boardStartX = boardPos.x - boardScale.x / 2;

        float targetX = (boardStartX + BoardPieceScale.x / 2) + (BoardPieceScale.x * index);

        Vector3 targetPos = new Vector3(targetX, boardPos.y + 0.1f, boardPos.z);

        return targetPos;
    }

    private void CheckMatch(Piece piece)
    {
        List<Piece> matchingPieces = new List<Piece>();
        foreach (var checkPiece in takenPieces)
        {
            if (checkPiece.type == piece.type)
                matchingPieces.Add(checkPiece);
        }

        if (matchingPieces.Count >= 3)
        {
            SetMatch(takenPieces.IndexOf(piece),matchingPieces);
        }
        else if (takenPieces.Count == maxTakenPieceCount)
        {
            LevelManager.Instance.OpenLosePanel();
        }
        
    }

    private void SetMatch(int pieceIndex,List<Piece> matchingPieces)
    {

        int order = 0;
        foreach (var piece in matchingPieces)
        {
            takenPieces.Remove(piece);
            piece.isOpen = false;
            StartCoroutine(piece.SetMatch(order));
            order++;
        }
        
        for (int i = 0; i < takenPieces.Count; i++)
        {
            StartCoroutine(SetNewPos(takenPieces[i],2,false));
        }
    }
    
}
