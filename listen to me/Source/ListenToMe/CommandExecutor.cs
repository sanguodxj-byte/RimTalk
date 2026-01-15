using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace ListenToMe
{
    /// <summary>
    /// 指令执行器 - 将解析后的指令转换为实际的游戏任务
    /// </summary>
    public static class CommandExecutor
    {
        /// <summary>
        /// 执行指令
        /// </summary>
        public static bool ExecuteCommand(ParsedCommand command, Pawn pawn)
        {
            if (pawn == null || pawn.Dead || pawn.Downed)
            {
                Messages.Message("? 无法执行指令：小人无法行动", MessageTypeDefOf.RejectInput);
                return false;
            }

            // 显示开始执行的消息
            Log.Message($"[ListenToMe] 开始执行指令: {command.OriginalText} | 类型: {command.Type} | 小人: {pawn.LabelShort}");

            try
            {
                bool success = false;
                string actionDescription = "";

                switch (command.Type)
                {
                    case CommandParser.CommandType.Move:
                        actionDescription = "移动";
                        success = ExecuteMove(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Work:
                        actionDescription = "工作";
                        success = ExecuteWork(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Fight:
                        actionDescription = "战斗";
                        success = ExecuteFight(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Wait:
                        actionDescription = "等待";
                        success = ExecuteWait(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Craft:
                        actionDescription = "制作";
                        success = ExecuteCraft(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Construct:
                        actionDescription = "建造";
                        success = ExecuteConstruct(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Haul:
                        actionDescription = "搬运";
                        success = ExecuteHaul(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Hunt:
                        actionDescription = "狩猎";
                        success = ExecuteHunt(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Gather:
                        actionDescription = "采集";
                        success = ExecuteGather(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Mine:
                        actionDescription = "挖矿";
                        success = ExecuteMine(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Tend:
                        actionDescription = "医疗";
                        success = ExecuteTend(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Clean:
                        actionDescription = "清洁";
                        success = ExecuteClean(command, pawn);
                        break;
                    
                    case CommandParser.CommandType.Repair:
                        actionDescription = "修理";
                        success = ExecuteRepair(command, pawn);
                        break;
                    
                    default:
                        Messages.Message($"? 未知的指令类型: {command.OriginalText}", MessageTypeDefOf.RejectInput);
                        Log.Warning($"[ListenToMe] 未知的指令类型: {command.Type}");
                        return false;
                }

                // 记录执行结果
                if (success)
                {
                    Log.Message($"[ListenToMe] ? 指令执行成功: {actionDescription} - {pawn.LabelShort}");
                    // 在小人头上显示Mote效果
                    ShowSuccessMote(pawn);
                }
                else
                {
                    Log.Warning($"[ListenToMe] ? 指令执行失败: {actionDescription} - {pawn.LabelShort}");
                }

                return success;
            }
            catch (Exception ex)
            {
                Log.Error($"[ListenToMe] ? 执行指令时出错: {ex.Message}\n{ex.StackTrace}");
                Messages.Message($"? 执行指令失败: {ex.Message}", MessageTypeDefOf.RejectInput);
                return false;
            }
        }

        /// <summary>
        /// 执行移动指令
        /// </summary>
        private static bool ExecuteMove(ParsedCommand command, Pawn pawn)
        {
            IntVec3 destination;
            
            if (command.Target != null)
            {
                // 移动到目标位置
                destination = command.Target.Position;
            }
            else if (command.TargetLocation.IsValid)
            {
                destination = command.TargetLocation;
            }
            else
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到目标位置", MessageTypeDefOf.RejectInput);
                return false;
            }

            // 创建移动任务
            Job job = JobMaker.MakeJob(JobDefOf.Goto, destination);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"? {pawn.LabelShort} 正在前往目标位置", MessageTypeDefOf.TaskCompletion);
            Log.Message($"[ListenToMe] {pawn.LabelShort} 移动到 {destination}");
            return true;
        }

        /// <summary>
        /// 执行工作指令 - 简化版，只移动到工作地点
        /// </summary>
        private static bool ExecuteWork(ParsedCommand command, Pawn pawn)
        {
            // 如果指定了工作台目标
            if (command.Target is Building_WorkTable workTable)
            {
                // 移动到工作台附近
                IntVec3 destination = workTable.InteractionCell;
                if (!destination.IsValid)
                {
                    destination = workTable.Position;
                }

                Job job = JobMaker.MakeJob(JobDefOf.Goto, destination);
                bool success = pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                
                if (success)
                {
                    Messages.Message($"? {pawn.LabelShort} 正在前往 {workTable.Label}", MessageTypeDefOf.TaskCompletion);
                    Log.Message($"[ListenToMe] {pawn.LabelShort} 移动到工作台: {workTable.Label}");
                    return true;
                }
                else
                {
                    Messages.Message($"? {pawn.LabelShort}: 无法前往工作台", MessageTypeDefOf.RejectInput);
                    return false;
                }
            }
            
            // 如果指定了房间位置
            if (command.TargetLocation.IsValid)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Goto, command.TargetLocation);
                bool success = pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                
                if (success)
                {
                    Messages.Message($"? {pawn.LabelShort} 正在前往工作地点", MessageTypeDefOf.TaskCompletion);
                    return true;
                }
            }
            
            // 未指定明确目标，提示用户
            Messages.Message($"? {pawn.LabelShort}: 请指定工作地点（例如：去厨房 / 到裁缝台）", MessageTypeDefOf.RejectInput);
            Log.Warning($"[ListenToMe] ExecuteWork: 未找到明确的工作目标");
            return false;
        }

        /// <summary>
        /// 执行战斗指令
        /// </summary>
        private static bool ExecuteFight(ParsedCommand command, Pawn pawn)
        {
            Pawn target = null;
            
            if (command.Target is Pawn targetPawn)
            {
                target = targetPawn;
            }
            else
            {
                // 查找最近的敌对目标
                target = FindNearestHostile(pawn);
            }
            
            if (target == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到攻击目标", MessageTypeDefOf.RejectInput);
                return false;
            }

            // 创建攻击任务
            Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, target);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"? {pawn.LabelShort} 开始攻击 {target.LabelShort}！", MessageTypeDefOf.TaskCompletion);
            Log.Message($"[ListenToMe] {pawn.LabelShort} 攻击 {target.LabelShort}");
            return true;
        }

        /// <summary>
        /// 执行等待指令
        /// </summary>
        private static bool ExecuteWait(ParsedCommand command, Pawn pawn)
        {
            // 清除当前任务
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            
            // 创建等待任务 - 使用 Wait_Combat 而不是 Wait
            Job job = JobMaker.MakeJob(JobDefOf.Wait_Combat);
            job.expiryInterval = 2500;  // 等待约40秒 (2500 ticks)
            job.checkOverrideOnExpire = false;
            
            // 尝试分配任务
            bool success = pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            if (success)
            {
                Messages.Message($"? {pawn.LabelShort} 开始等待", MessageTypeDefOf.TaskCompletion);
                Log.Message($"[ListenToMe] {pawn.LabelShort} 成功开始等待任务，持续 {job.expiryInterval} ticks");
            }
            else
            {
                Messages.Message($"? {pawn.LabelShort}: 无法执行等待", MessageTypeDefOf.RejectInput);
                Log.Warning($"[ListenToMe] {pawn.LabelShort} 无法接受等待任务");
            }
            
            return success;
        }

        /// <summary>
        /// 执行制作指令
        /// </summary>
        private static bool ExecuteCraft(ParsedCommand command, Pawn pawn)
        {
            // 查找工作台
            Building_WorkTable workTable = null;
            
            if (command.Target is Building_WorkTable wt)
            {
                workTable = wt;
            }
            else if (command.ItemToCraft != null)
            {
                // 根据物品查找合适的工作台
                workTable = FindWorkTableForRecipe(pawn, command.ItemToCraft);
            }
            
            if (workTable == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到合适的工作台", MessageTypeDefOf.RejectInput);
                return false;
            }

            // 查找或创建配方
            if (command.ItemToCraft != null)
            {
                var recipe = FindOrCreateRecipe(workTable, command.ItemToCraft, command.Count);
                if (recipe != null)
                {
                    Messages.Message($"?? {pawn.LabelShort} 将制作 {command.Count} 个 {command.ItemToCraft.label}", 
                        MessageTypeDefOf.TaskCompletion);
                    Log.Message($"[ListenToMe] {pawn.LabelShort} 制作 {command.Count}x {command.ItemToCraft.label}");
                    
                    // 执行制作任务
                    Job job = JobMaker.MakeJob(JobDefOf.DoBill, workTable);
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    return true;
                }
            }
            
            Messages.Message($"? {pawn.LabelShort}: 无法创建制作任务", MessageTypeDefOf.RejectInput);
            return false;
        }

        /// <summary>
        /// 执行建造指令
        /// </summary>
        private static bool ExecuteConstruct(ParsedCommand command, Pawn pawn)
        {
            if (command.Target is Frame frame)
            {
                Job job = JobMaker.MakeJob(JobDefOf.FinishFrame, frame);
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                Messages.Message($"?? {pawn.LabelShort} 开始建造 {frame.Label}", MessageTypeDefOf.TaskCompletion);
                return true;
            }
            
            if (command.Target is Blueprint blueprint)
            {
                Job job = JobMaker.MakeJob(JobDefOf.PlaceNoCostFrame, blueprint);
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                Messages.Message($"?? {pawn.LabelShort} 开始建造", MessageTypeDefOf.TaskCompletion);
                return true;
            }
            
            // 查找最近的建造框架
            var nearestFrame = FindNearestFrame(pawn);
            if (nearestFrame != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.FinishFrame, nearestFrame);
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                Messages.Message($"?? {pawn.LabelShort} 开始建造", MessageTypeDefOf.TaskCompletion);
                return true;
            }
            
            Messages.Message($"? {pawn.LabelShort}: 未找到建造目标", MessageTypeDefOf.RejectInput);
            return false;
        }

        /// <summary>
        /// 执行搬运指令
        /// </summary>
        private static bool ExecuteHaul(ParsedCommand command, Pawn pawn)
        {
            Thing thing = command.Target;
            
            if (thing == null)
            {
                // 查找最近的需要搬运的物品
                thing = FindNearestHaulable(pawn);
            }
            
            if (thing == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到需要搬运的物品", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.HaulToCell, thing);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始搬运 {thing.Label}", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行狩猎指令
        /// </summary>
        private static bool ExecuteHunt(ParsedCommand command, Pawn pawn)
        {
            Pawn prey = null;
            
            if (command.Target is Pawn targetPawn && targetPawn.RaceProps.Animal)
            {
                prey = targetPawn;
            }
            else
            {
                // 查找最近的可狩猎动物
                prey = FindNearestHuntable(pawn);
            }
            
            if (prey == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到狩猎目标", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Hunt, prey);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始狩猎 {prey.LabelShort}", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行采集指令
        /// </summary>
        private static bool ExecuteGather(ParsedCommand command, Pawn pawn)
        {
            // 查找最近的可采集植物
            var plant = FindNearestHarvestable(pawn);
            
            if (plant == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到可采集的植物", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Harvest, plant);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始采集 {plant.Label}", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行挖矿指令
        /// </summary>
        private static bool ExecuteMine(ParsedCommand command, Pawn pawn)
        {
            Thing mineable = null;
            
            if (command.Target is Mineable m)
            {
                mineable = m;
            }
            else
            {
                mineable = FindNearestMineable(pawn);
            }
            
            if (mineable == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到可挖掘的矿石", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Mine, mineable);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"? {pawn.LabelShort} 开始挖矿", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行医疗指令
        /// </summary>
        private static bool ExecuteTend(ParsedCommand command, Pawn pawn)
        {
            Pawn patient = null;
            
            if (command.Target is Pawn targetPawn)
            {
                patient = targetPawn;
            }
            else
            {
                patient = FindNearestWounded(pawn);
            }
            
            if (patient == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到需要治疗的对象", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.TendPatient, patient);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始治疗 {patient.LabelShort}", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行清洁指令
        /// </summary>
        private static bool ExecuteClean(ParsedCommand command, Pawn pawn)
        {
            var filth = FindNearestFilth(pawn);
            
            if (filth == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到需要清洁的区域", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Clean, filth);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始清洁", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        /// <summary>
        /// 执行修理指令
        /// </summary>
        private static bool ExecuteRepair(ParsedCommand command, Pawn pawn)
        {
            Thing damaged = null;
            
            if (command.Target != null && command.Target.HitPoints < command.Target.MaxHitPoints)
            {
                damaged = command.Target;
            }
            else
            {
                damaged = FindNearestDamaged(pawn);
            }
            
            if (damaged == null)
            {
                Messages.Message($"? {pawn.LabelShort}: 未找到需要修理的物品", MessageTypeDefOf.RejectInput);
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Repair, damaged);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            
            Messages.Message($"?? {pawn.LabelShort} 开始修理 {damaged.Label}", MessageTypeDefOf.TaskCompletion);
            return true;
        }

        // ========== 辅助方法 ==========

        private static Pawn FindNearestHostile(Pawn pawn)
        {
            return pawn.Map?.mapPawns.AllPawns
                .Where(p => p.HostileTo(pawn.Faction) && !p.Dead && !p.Downed)
                .OrderBy(p => p.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Building_WorkTable FindWorkTableForRecipe(Pawn pawn, ThingDef product)
        {
            var allRecipes = DefDatabase<RecipeDef>.AllDefsListForReading
                .Where(r => r.products != null && r.products.Any(p => p.thingDef == product));
            
            foreach (var recipe in allRecipes)
            {
                var workTable = pawn.Map?.listerBuildings.allBuildingsColonist
                    .OfType<Building_WorkTable>()
                    .Where(wt => wt.def.AllRecipes != null && wt.def.AllRecipes.Contains(recipe))
                    .OrderBy(wt => wt.Position.DistanceTo(pawn.Position))
                    .FirstOrDefault();
                
                if (workTable != null)
                    return workTable;
            }
            
            return null;
        }

        private static Bill FindOrCreateRecipe(Building_WorkTable workTable, ThingDef product, int count)
        {
            var recipe = DefDatabase<RecipeDef>.AllDefsListForReading
                .FirstOrDefault(r => r.products != null && 
                               r.products.Any(p => p.thingDef == product) &&
                               workTable.def.AllRecipes != null &&
                               workTable.def.AllRecipes.Contains(r));
            
            if (recipe == null)
                return null;

            // 检查是否已有相同的配方
            var existingBill = workTable.BillStack?.Bills
                .FirstOrDefault(b => b.recipe == recipe);
            
            if (existingBill != null)
            {
                if (existingBill is Bill_Production productionBill)
                {
                    productionBill.repeatCount = count;
                }
                return existingBill;
            }

            // 创建新配方
            Bill_Production newBill = (Bill_Production)recipe.MakeNewBill();
            newBill.repeatMode = BillRepeatModeDefOf.RepeatCount;
            newBill.repeatCount = count;
            workTable.BillStack?.AddBill(newBill);
            
            return newBill;
        }

        private static Thing FindNearestFrame(Pawn pawn)
        {
            return pawn.Map?.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
                .OrderBy(t => t.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Thing FindNearestHaulable(Pawn pawn)
        {
            return pawn.Map?.listerHaulables.ThingsPotentiallyNeedingHauling()
                .OrderBy(t => t.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Pawn FindNearestHuntable(Pawn pawn)
        {
            return pawn.Map?.mapPawns.AllPawns
                .Where(p => p.RaceProps.Animal && 
                           !p.Dead && 
                           p.Faction == null &&
                           p.RaceProps.IsFlesh)
                .OrderBy(p => p.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Plant FindNearestHarvestable(Pawn pawn)
        {
            return pawn.Map?.listerThings.ThingsInGroup(ThingRequestGroup.HarvestablePlant)
                .OfType<Plant>()
                .Where(p => p.HarvestableNow)
                .OrderBy(p => p.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Thing FindNearestMineable(Pawn pawn)
        {
            return pawn.Map?.listerThings.ThingsInGroup(ThingRequestGroup.Undefined)
                .Where(t => t is Mineable)
                .OrderBy(t => t.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Pawn FindNearestWounded(Pawn pawn)
        {
            return pawn.Map?.mapPawns.AllPawns
                .Where(p => p.health.HasHediffsNeedingTend() && p != pawn)
                .OrderBy(p => p.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private static Filth FindNearestFilth(Pawn pawn)
        {
            if (pawn.Map == null) return null;
            
            var filthList = pawn.Map.listerFilthInHomeArea?.FilthInHomeArea;
            if (filthList == null || filthList.Count == 0) return null;
            
            Filth nearestFilth = null;
            float nearestDist = float.MaxValue;
            
            foreach (var f in filthList)
            {
                if (f is Filth filth)
                {
                    float dist = f.Position.DistanceTo(pawn.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestFilth = filth;
                    }
                }
            }
            
            return nearestFilth;
        }

        private static Thing FindNearestDamaged(Pawn pawn)
        {
            return pawn.Map?.listerThings.AllThings
                .Where(t => t.HitPoints < t.MaxHitPoints && t.def.useHitPoints)
                .OrderBy(t => t.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// 在小人头上显示成功标记
        /// </summary>
        private static void ShowSuccessMote(Pawn pawn)
        {
            try
            {
                if (pawn?.Map == null) return;
                
                // 显示绿色的成功图标
                MoteMaker.ThrowText(pawn.DrawPos + new Vector3(0f, 0f, 0.5f), pawn.Map, "?", Color.green, 3.5f);
            }
            catch (Exception ex)
            {
                Log.Warning($"[ListenToMe] 显示Mote失败: {ex.Message}");
            }
        }
    }
}
