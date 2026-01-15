using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace DiningEtiquette
{
    public class JobGiver_DiningEtiquette : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable == null)
            {
                return 0f;
            }
            if (pawn.timetable.CurrentAssignment == DE_DefOf.DiningEtiquette_MealTime)
            {
                return 9.5f; // 高优先级，高于普通Work (9) 但低于紧急事件
            }
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.timetable == null || pawn.timetable.CurrentAssignment != DE_DefOf.DiningEtiquette_MealTime)
            {
                return null;
            }
            
            // 如果已经被征召，或者无法进食，则跳过
            if (pawn.Drafted || !pawn.RaceProps.Humanlike)
            {
                return null;
            }

            Need_Food foodNeed = pawn.needs.food;
            if (foodNeed == null)
            {
                return null;
            }

            // 1. 极度饥饿判定 (Priority 0 - Emergency)
            // 如果小人处于 UrgentlyHungry (0.15) 或 Starving (0.01) 状态，
            // 必须无视一切礼仪，直接吃正餐（复用原版逻辑）。
            if (foodNeed.CurCategory >= HungerCategory.UrgentlyHungry)
            {
                return GetStandardFoodJob(pawn);
            }

            // 2. 正常饥饿判定 (Priority 1) - 阈值 B (50%) 以下
            // 逻辑: 50%以下，优先寻找正餐并食用
            if (foodNeed.CurLevelPercentage < DiningEtiquetteMod.settings.mealHungerThreshold)
            {
                Job foodJob = GetStandardFoodJob(pawn);
                if (foodJob != null)
                {
                    return foodJob;
                }
                // 如果找不到正餐，通常原版逻辑会处理，或者我们可以尝试下面的零食逻辑作为备选
                // 但根据用户要求，50%以下主要是找正餐。
            }

            // 3. 微饿判定 (Priority 2) - 阈值 B (50%) ~ 阈值 A (95%)
            // 逻辑: 在周围10格半径扫描寻找零食和饮料，不存在也不进行动作。存在则拿取并到最近的桌椅就餐。
            if (foodNeed.CurLevelPercentage >= DiningEtiquetteMod.settings.mealHungerThreshold &&
                foodNeed.CurLevelPercentage < DiningEtiquetteMod.settings.highSaturationThreshold)
            {
                // 限制搜索半径为 10 格
                Job snackJob = GetSnackJob(pawn, 10f);
                if (snackJob != null)
                {
                    return snackJob;
                }
                
                // 不存在也不进行动作 -> 返回 null，继续手头工作
                return null;
            }

            // 4. 饱腹判定 (Priority 3) - 阈值 A (95%) 以上
            // 逻辑: 默认继续手头的工作不进行变化。
            // 配置项: 让用户决定是否也要到桌椅上进行社交放松。
            if (foodNeed.CurLevelPercentage >= DiningEtiquetteMod.settings.highSaturationThreshold)
            {
                if (DiningEtiquetteMod.settings.allowSocializeAtHighSaturation)
                {
                     // 用户开启了饱腹社交，寻找座位
                     return GetSocialJob(pawn);
                }
                else
                {
                    // 继续手头工作 -> 返回 null
                    return null;
                }
            }

            return null;
        }

        private Job GetStandardFoodJob(Pawn pawn)
        {
            // 使用原版的 JobGiver_GetFood 逻辑
            // 由于 TryGiveJob 是 protected，我们需要通过 TryIssueJobPackage 调用
            JobGiver_GetFood jobGiver = new JobGiver_GetFood();
            // 确保能搜索全图
            jobGiver.forceScanWholeMap = true;
            
            // 调用 TryIssueJobPackage (它会内部调用 TryGiveJob)
            // JobIssueParams 通常可以传默认值
            var result = jobGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
            return result.Job;
        }

        private Job GetSnackJob(Pawn pawn, float radius)
        {
            // 搜索地图上的热饮或茶点
            Thing snack = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree),
                PathEndMode.ClosestTouch,
                TraverseParms.For(pawn),
                radius, // 使用传入的半径限制
                (Thing t) => IsSnack(t) && !t.IsForbidden(pawn) && pawn.CanReserve(t)
            );

            if (snack != null)
            {
                // 计算需要多少个（通常零食很小，吃一个即可，或者根据营养计算）
                // 热饮 0.05，茶点 0.15。
                // 如果小人只缺 0.1，吃一个热饮正好。
                // 这里的逻辑简化为：只要能吃且不会严重溢出，就吃。
                
                // JobDef: Ingest
                Job job = JobMaker.MakeJob(JobDefOf.Ingest, snack);
                job.count = 1; // 这是一个简单的下午茶，只吃一份
                return job;
            }

            return null;
        }

        private bool IsSnack(Thing t)
        {
            if (t.def == DE_DefOf.DE_HotDrink || t.def == DE_DefOf.DE_ColdDrink ||
                t.def == DE_DefOf.DE_SnackSimple || t.def == DE_DefOf.DE_SnackFine || t.def == DE_DefOf.DE_SnackLavish)
            {
                return true;
            }
            return false;
        }

        private Job GetSocialJob(Pawn pawn)
        {
            // 寻找聚会点 (Table)
            // 类似于 JobGiver_WanderColony 或 JobGiver_GetJoyInBed，
            // 但我们希望他们聚集在餐厅。
            
            // 尝试找到最近的拥有 GatherSpot 的桌子
            Building table = FindGatheringTable(pawn);
            if (table != null)
            {
                // 找到桌子边的椅子
                IntVec3 chairPos = IntVec3.Invalid;
                foreach (IntVec3 adj in GenAdj.CellsAdjacent8Way(table))
                {
                    
                    if (adj.InBounds(pawn.Map) &&
                        adj.Standable(pawn.Map) && // 简单的可达性检查
                        pawn.CanReserveAndReach(adj, PathEndMode.OnCell, Danger.None))
                    {
                        // 检查是否有椅子（可选，但有椅子更好）
                        // 简化：只要能站立且在桌子旁即可
                         chairPos = adj;
                         break;
                    }
                }

                if (chairPos.IsValid)
                {
                    // 这里的 Job 实际上应该是一个持续一段时间的“待机”或“观看”任务
                    // 原版没有直接的 "SitAndChat" Job 给玩家用（SocialRelax 是 JoyGiver 触发的）
                    // 我们可以给一个 "Wait_Wander" 或者简单的 "Goto" 然后待着
                    // 或者更高级：使用 JobDefOf.SocialRelax 如果能触发的话
                    
                    // 为了简单起见，且确保兼容性，我们可以让他们去桌子旁“观看”或“发呆”
                    // 更好的做法是实现一个自定义 JobDriver_SitAndChat，但这需要更多的 XML 定义。
                    // 暂时使用 GoTo + Wait
                    
                    // 实际上，我们可以利用 JobGiver_WanderCurrentRoom，如果已经在餐厅的话。
                    // 但为了强制去餐厅：
                    
                    return JobMaker.MakeJob(JobDefOf.GotoWander, chairPos);
                }
            }
            
            // 如果找不到桌子，就在基地附近闲逛
            // JobGiver_Wander.TryGiveJob 是 protected，我们需要使用 TryIssueJobPackage
            var wanderGiver = new JobGiver_WanderColony();
            return wanderGiver.TryIssueJobPackage(pawn, default(JobIssueParams)).Job;
        }

        private Building FindGatheringTable(Pawn pawn)
        {
            // 查找地图上所有的桌子，优先找被标记为 GatherSpot 的
            // 注意：RimWorld 中没有 Building_Table 类，桌子通常是 Building_WorkTable 或普通 Building
            // 这里我们通过 ThingDef 的 surfaceType 来判断是否为桌子 (SurfaceType.Eat)
            List<Building> allBuildings = pawn.Map.listerBuildings.allBuildingsColonist;
            Building bestTable = null;
            float bestDist = float.MaxValue;

            foreach (Building b in allBuildings)
            {
                if (b.def.surfaceType == SurfaceType.Eat)
                {
                    // 优先选择聚会点
                    // 1.5版本中 gatherSpot 是一个字段，而不是 isGatherSpot
                    // 1.5版本中 gatherSpot 可能不存在或不同。
                    // 实际上 ThingDef.BuildingProperties 中没有 gatherSpot 字段，
                    // 而是通过 GatherSpotDef 定义或者直接检查是否有 CompGatherSpot 组件。
                    // 但通常 WorkTable 或 Table 都有 CompGatherSpot。
                    // 或者我们直接假定所有 SurfaceType.Eat 且是 GatherSpot 的都是桌子。
                    
                    // 实际上原版是通过 Building.IsGatherSpot 来判断的吗？
                    // 不，Building 没有 IsGatherSpot。
                    
                    // 正确的方法是检查 CompGatherSpot
                    if (b.TryGetComp<CompGatherSpot>() != null && b.TryGetComp<CompGatherSpot>().Active)
                    {
                        {
                            float dist = (b.Position - pawn.Position).LengthHorizontal;
                            if (dist < bestDist && pawn.CanReach(b, PathEndMode.Touch, Danger.None))
                            {
                                bestTable = b;
                                bestDist = dist;
                            }
                        }
                    }
                }
            }
            
            // 如果没有 GatherSpot，随便找个桌子
            if (bestTable == null)
            {
                 foreach (Building b in allBuildings)
                {
                    if (b.def.surfaceType == SurfaceType.Eat)
                    {
                        float dist = (b.Position - pawn.Position).LengthHorizontal;
                        if (dist < bestDist && pawn.CanReach(b, PathEndMode.Touch, Danger.None))
                        {
                            bestTable = b;
                            bestDist = dist;
                        }
                    }
                }
            }

            return bestTable;
        }
    }
}