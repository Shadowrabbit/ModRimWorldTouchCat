﻿// ******************************************************************
//       /\ /|       @file       JobDriverTouchCat.cs
//       \ V/        @brief      行为 撸猫
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-25 17:23:36
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace SR.ModRimWorldTouchCat
{
    [UsedImplicitly]
    public class JobDriverTouchCat : JobDriverTouchPet
    {
        /// <summary>
        /// 全部步骤成功时回调
        /// </summary>
        protected override void OnToilsSuccess()
        {
            //触发撸猫的回忆
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SrThoughtTouchCat);
            //概率触发成瘾	
            CalcAddiction();
            //概率加入殖民者阵营
            CalcJoin();
            //计算需求
            CalcNeedTouchCat();
            //增加驯兽经验
            pawn.skills.Learn(SkillDefOf.Animals, XpSkillAnimalLearn);
        }

        /// <summary>
        /// 计算成瘾触发
        /// </summary>
        private void CalcAddiction()
        {
            //已经成瘾 不会重复触发
            if (Enumerable.Any(pawn.health.hediffSet.hediffs,
                heddif => heddif.def == HediffDefOf.SrHediffAddictionTouchCat))
            {
                return;
            }

            var randomNum = Random.Range(0f, 1f);
            if ((randomNum > ChanceToAddiction))
            {
                return;
            }

            var hediffAddictionTouchCat = HediffMaker.MakeHediff(HediffDefOf.SrHediffAddictionTouchCat, pawn);
            pawn.health.AddHediff(hediffAddictionTouchCat);
            //玩家阵营的小人上瘾会提示
            if (pawn.Faction == Faction.OfPlayer)
            {
                Messages.Message("MsgAddictionTouchCat".Translate(pawn.Label), MessageTypeDefOf.NeutralEvent);
            }
        }

        /// <summary>
        /// 计算加入殖民者
        /// </summary>
        private void CalcJoin()
        {
            //阵营相同
            if (Pet.Faction == pawn.Faction)
            {
                return;
            }

            //羁绊动物
            if (Pet.playerSettings?.Master != null)
            {
                return;
            }
            
            var randomNum = Random.Range(0f, 1f);
            if ((randomNum > ChanceToJoin))
            {
                return;
            }

            Pet.SetFaction(pawn.Faction);
            Messages.Message("MsgTouchPetJoin".Translate(pawn.Label, Pet.Label, pawn.Faction),
                MessageTypeDefOf.NeutralEvent);
        }

        /// <summary>
        /// 计算撸猫需求
        /// </summary>
        private void CalcNeedTouchCat()
        {
            //如果存在撸猫需求 恢复到最高水平
            var allNeeds = pawn.needs.AllNeeds;
            var needTouchCat = allNeeds.Where(t => t.def == NeedDefOf.SrNeedTouchCat).Cast<NeedTouchPet>()
                .FirstOrDefault();
            //当前角色不存在撸猫需求
            if (needTouchCat == null)
            {
                return;
            }

            needTouchCat.CurLevel = 1f;
        }
    }
}