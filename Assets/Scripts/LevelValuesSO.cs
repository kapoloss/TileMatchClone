using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Level/LevelValues")]
public class LevelValuesSO : ScriptableObject
{

    public Vector3 pieceScale;

    [Range(0, 15)]
    public int defEmptyPercent;

    [Range(1, 4)]
    public int layerCount;
    public int pieceKindCount;
    public int neededPieceForMatch;

    public List<PieceProperties> pieceProperties;

    private void OnValidate()
    {
        if (pieceProperties.Count < pieceKindCount)
        {
            int difference = pieceKindCount - pieceProperties.Count;
            for (int i = 0; i < difference; i++)
            {
                pieceProperties.Add(new PieceProperties());
            }
        }
        else if (pieceProperties.Count > pieceKindCount)
        {
            int difference = pieceProperties.Count - pieceKindCount;
            pieceProperties.RemoveRange(pieceKindCount, difference);
        }
    }

}

[System.Serializable]
public struct PieceProperties
{
    public Sprite image;
}
