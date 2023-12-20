using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Lofelt.NiceVibrations;
public class Piece : MonoBehaviour
{
    public int type;

    public Vector3Int arrayPos;
    public List<Piece> upNeighbours;
    public List<Piece> downNeighbours;

    
    public GameObject mesh;
    public GameObject canvas;
    public Image mainImage;
    public Image unavailableImage;
    public bool isOpen = true;
    public bool isBlocked;
    public bool isTaken;
    

    // Start is called before the first frame update
    void Start()
    {
        SetOpen();
        SetAvailable();
    }

    public void SetOpen()
    {
        if (!isOpen)
        {
             gameObject.SetActive(false);
        }
    }

    public void SetAvailable()
    {
        bool isAvailable = CheckAvailable();
        //Debug.Log(isAvailable);
        if (isAvailable)
        {
            unavailableImage.gameObject.SetActive(false);
            isBlocked = false;
        }
        else
        {
            unavailableImage.gameObject.SetActive(true);
            isBlocked = true;
        }
    }

    public bool CheckAvailable()
    {
        bool isAvailable = true;
        
        if (isOpen && !isTaken)
        {
            foreach (var neighbour in upNeighbours)
            {
                if (neighbour.isOpen && !neighbour.isTaken)
                {
                    isAvailable = false;
                }
            }
        }
        return isAvailable;

    }

    // public void OpenPiece()
    // {
    //     mesh.SetActive(true);
    //     canvas.SetActive(true);
    // }
    public void ClosePiece()
    {
        mesh.SetActive(false);
        canvas.SetActive(false);
    }

    public IEnumerator SetMatch(int order)
    {
        float timeDif = 0.1f;
        
        yield return new WaitForSeconds(timeDif * order);
        
        Vector3 startScale = transform.localScale;
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        {
            ScoreSystem.Instance.SendStar(transform.position);
            transform.localScale = startScale;
            ClosePiece();
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
            LevelManager.Instance.currentGridSystem.allPieces.Remove(this);
        });
    }
}
