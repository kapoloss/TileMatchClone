using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "ScriptableObjects/SkillAction/DestroySkill")]
public class DestroySkill : BaseSkillActionSO
{
   public int destroyMatchCount;
   private GridSystem gridSystem;

   public override void Skill()
   {
      gridSystem = LevelManager.Instance.currentGridSystem;

      for (int i = 0; i < destroyMatchCount; i++)
      {
         int matchPieceIndex = gridSystem.allPieces[^1].type;

         List<Piece> matchPieces = new List<Piece>();

         for (int j = gridSystem.allPieces.Count-1; j > 0; j--)
         {
            Piece piece = gridSystem.allPieces[j];
            
            if (piece.type == matchPieceIndex && piece.isOpen && !piece.isTaken)
            {
               matchPieces.Add(piece);
            }
         }

         if (matchPieces.Count >= gridSystem.neededMatchCount)
         {
            int order = 0;
            
            for (int k = 0; k < gridSystem.neededMatchCount + 1; k++)
            {
               Piece piece = matchPieces[k];
               
               piece.isOpen = false;
               GameControl.Instance.StartCoroutine(piece.SetMatch(order));
               order++;

               foreach (var downPiece in piece.downNeighbours)
               {
                  downPiece.SetAvailable();
               }
            }
            
         }

      }
   }
   
}
