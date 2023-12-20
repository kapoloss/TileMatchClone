using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SkillAction/RestartSkill")]
public class RestartSkill : BaseSkillActionSO
{
   public override void Skill()
   {
      LevelManager.Instance.Restart();
   }
}
