using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public Vector3 levelAreaScale;
    public Vector3 pieceScale;
    public int layerCount;
    public int pieceKindCount;
    public float defEmptyPercent;
    public List<PieceProperties> pieceProperties;
    public int neededMatchCount;
    public List<int> isOpenArray;
}
