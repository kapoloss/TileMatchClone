using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SkillAction/UndoSkill")]
public class UndoSkill : BaseSkillActionSO
{
   public override void Skill()
   {
      if (GameControl.Instance.takenPieces.Count > 0)
      {
         Piece lastTakenPiece = GameControl.Instance.takenPieces[^1];

         Vector3 piecesPos = GameControl.Instance.FindPiecePosition(lastTakenPiece);

         GameControl.Instance.StartCoroutine(SendPieceBack(lastTakenPiece, piecesPos));

         lastTakenPiece.isTaken = false;
         lastTakenPiece.isOpen = true;

         foreach (var downPiece in lastTakenPiece.downNeighbours)
         {
            downPiece.SetAvailable();
         }

         GameControl.Instance.takenPieces.Remove(lastTakenPiece);
      }
     
   }

   public IEnumerator SendPieceBack(Piece piece,Vector3 targetPos)
   {
      while (Vector3.Distance(piece.transform.position, targetPos) > 0.01f)
      {
         piece.transform.position = Vector3.MoveTowards(piece.transform.position,
            targetPos, Time.deltaTime * 10);
         yield return null;
      }
   }
}
