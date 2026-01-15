using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace CulinaryArts.Harmony
{
    /// <summary>
    /// 食物摄入补丁 v3.1
    /// 功能1：拦截并删除原版记忆（防止双重心情）
    /// 功能2：主动向 RimTalk 注入精确的菜名记忆
    /// </summary>
    [HarmonyPatch(typeof(Thing), "Ingested")]
    public static class Patch_Thing_Ingested
    {
        /// <summary>
        /// 静态缓存类，避免重复反射查找
        /// </summary>
        private static class RimTalkReflectionCache
        {
            public static bool IsInitialized = false;
            public static bool IsRimTalkActive = false;

            public static Type FourLayerMemoryCompType;
            public static Type PawnMemoryCompType;
            public static MethodInfo AddActiveMemoryMethod;
            public static MethodInfo AddMemoryMethod;
            
            public static Type MemoryTypeEnum;
            public static object ActionType;
            
            public static PropertyInfo ActiveMemoriesProp;
            public static Type MemoryEntryType;
            public static FieldInfo ContentField;
            public static FieldInfo TypeField;
            public static FieldInfo TimestampField;

            public static void EnsureInitialized()
            {
                if (IsInitialized) return;

                try
                {
                    FourLayerMemoryCompType = AccessTools.TypeByName("RimTalk.Memory.FourLayerMemoryComp");
                    PawnMemoryCompType = AccessTools.TypeByName("RimTalk.Memory.PawnMemoryComp");
                    
                    // 只要找到任意一个组件类型，就认为 RimTalk 可能存在
                    Type compType = FourLayerMemoryCompType ?? PawnMemoryCompType;
                    
                    if (compType != null)
                    {
                        AddActiveMemoryMethod = AccessTools.Method(compType, "AddActiveMemory");
                        AddMemoryMethod = AccessTools.Method(compType, "AddMemory");
                        ActiveMemoriesProp = AccessTools.Property(compType, "ActiveMemories");

                        MemoryTypeEnum = AccessTools.TypeByName("RimTalk.Memory.MemoryType");
                        if (MemoryTypeEnum != null)
                        {
                            try { ActionType = Enum.Parse(MemoryTypeEnum, "Action"); } catch { }
                        }

                        MemoryEntryType = AccessTools.TypeByName("RimTalk.Memory.MemoryEntry");
                        if (MemoryEntryType != null)
                        {
                            ContentField = AccessTools.Field(MemoryEntryType, "content");
                            TypeField = AccessTools.Field(MemoryEntryType, "type");
                            TimestampField = AccessTools.Field(MemoryEntryType, "timestamp");
                        }

                        // 只有关键字段都找到才标记为活跃
                        IsRimTalkActive = (AddActiveMemoryMethod != null || AddMemoryMethod != null) && 
                                          MemoryTypeEnum != null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"[CulinaryArts] RimTalk reflection cache initialization warning: {ex.Message}");
                    IsRimTalkActive = false;
                }
                finally
                {
                    IsInitialized = true;
                }
            }
        }

        /// <summary>
        /// 缓存 ThoughtDef 以减少 DefDatabase 查找开销
        /// </summary>
        private static class ThoughtDefCache
        {
            public static bool IsInitialized = false;

            public static ThoughtDef AteTerribleMeal;
            public static ThoughtDef AteLegendaryMeal;
            public static ThoughtDef AteGoodMeal;
            public static ThoughtDef AteNormalMeal;

            public static ThoughtDef[] VanillaFoodThoughts;

            public static void EnsureInitialized()
            {
                if (IsInitialized) return;

                AteTerribleMeal = DefDatabase<ThoughtDef>.GetNamedSilentFail("CulinaryArts_AteTerribleMeal");
                AteLegendaryMeal = DefDatabase<ThoughtDef>.GetNamedSilentFail("CulinaryArts_AteLegendaryMeal");
                AteGoodMeal = DefDatabase<ThoughtDef>.GetNamedSilentFail("CulinaryArts_AteGoodMeal");
                AteNormalMeal = DefDatabase<ThoughtDef>.GetNamedSilentFail("CulinaryArts_AteNormalMeal");

                var vanillaNames = new[]
                {
                    "Ate_MealLavish",
                    "Ate_MealFine",
                    "Ate_MealSimple",
                    "Ate_MealAwful",
                    "AteRawFood",
                    "AteWithoutTable"
                };

                var list = new System.Collections.Generic.List<ThoughtDef>();
                foreach (var name in vanillaNames)
                {
                    var def = DefDatabase<ThoughtDef>.GetNamedSilentFail(name);
                    if (def != null) list.Add(def);
                }
                VanillaFoodThoughts = list.ToArray();

                IsInitialized = true;
            }
        }

        public class IngestState
        {
            public string CustomName;
            public int MoodOffset;
            public ThoughtDef VanillaThoughtDef; // 记录原版记忆以便删除
        }

        [HarmonyPrefix]
        public static void Prefix(Thing __instance, Pawn ingester, ref IngestState __state)
        {
            var comp = __instance?.TryGetComp<CompNamedMeal>();
            // 预先捕获该食物原本会产生的记忆定义
            ThoughtDef vanillaDef = __instance?.def?.ingestible?.tasteThought;

            if (comp != null && !string.IsNullOrEmpty(comp.CustomName))
            {
                __state = new IngestState
                {
                    CustomName = comp.CustomName,
                    MoodOffset = comp.MoodOffset,
                    VanillaThoughtDef = vanillaDef
                };
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Pawn ingester, IngestState __state)
        {
            if (__state == null || ingester == null) return;

            // === 功能模块 1: RimTalk 记忆管理 ===
            // 1.1 先删除可能存在的原版食物记忆
            RemoveRimTalkVanillaFoodMemory(ingester);
            
            // 1.2 注入自定义菜名记忆
            if (!string.IsNullOrEmpty(__state.CustomName))
            {
                InjectRimTalkMemory(ingester, __state.CustomName);
            }

            // === 功能模块 2: 游戏内心情与记忆管理 ===
            if (ingester.needs?.mood?.thoughts?.memories != null)
            {
                var memoryHandler = ingester.needs.mood.thoughts.memories;

                try
                {
                    ThoughtDefCache.EnsureInitialized();

                    // A. 删除原版记忆 (Clean Up)
                    
                    // 首先尝试删除指定的原版记忆
                    if (__state.VanillaThoughtDef != null)
                    {
                        var oldMemory = memoryHandler.Memories.Find(x => x.def == __state.VanillaThoughtDef && x.age < 60);
                        if (oldMemory != null)
                        {
                            memoryHandler.RemoveMemory(oldMemory);
                        }
                    }

                    // 额外删除所有可能的原版食物记忆
                    // 使用缓存的 Def 列表，避免重复查找和数组分配
                    foreach (var thoughtDef in ThoughtDefCache.VanillaFoodThoughts)
                    {
                        var oldMemory = memoryHandler.Memories.Find(x => x.def == thoughtDef && x.age < 60);
                        if (oldMemory != null)
                        {
                            memoryHandler.RemoveMemory(oldMemory);
                        }
                    }

                    // B. 添加自定义记忆 (Add New)
                    // 根据心情值选择不同的记忆类型（用于显示正确的标签和描述）
                    ThoughtDef targetDef;
                    if (__state.MoodOffset < 0)
                    {
                        targetDef = ThoughtDefCache.AteTerribleMeal;
                    }
                    else if (__state.MoodOffset >= 15)
                    {
                        targetDef = ThoughtDefCache.AteLegendaryMeal;
                    }
                    else if (__state.MoodOffset >= 8)
                    {
                        targetDef = ThoughtDefCache.AteGoodMeal;
                    }
                    else
                    {
                        targetDef = ThoughtDefCache.AteNormalMeal;
                    }

                    if (targetDef != null)
                    {
                        var memory = (Thought_Memory_NamedMeal)ThoughtMaker.MakeThought(targetDef);
                        memory.dishName = __state.CustomName;
                        memory.customMoodOffset = __state.MoodOffset; // 使用实际计算的心情值
                        memoryHandler.TryGainMemory(memory);
                    }
                }
                catch (Exception ex)
                {
                    if (CulinaryArtsSettings.ShowDebugLog) Log.Error($"[CulinaryArts] Postfix Error: {ex}");
                }
            }
        }

        /// <summary>
        /// RimTalk 兼容性桥接方法 - 注入自定义菜名记忆
        /// </summary>
        private static void InjectRimTalkMemory(Pawn pawn, string dishName)
        {
            try
            {
                RimTalkReflectionCache.EnsureInitialized();
                if (!RimTalkReflectionCache.IsRimTalkActive) return;

                // 1. 查找组件
                Type compType = RimTalkReflectionCache.FourLayerMemoryCompType ?? RimTalkReflectionCache.PawnMemoryCompType;
                if (compType == null) return;

                var comp = pawn.AllComps.Find(c => compType.IsAssignableFrom(c.GetType()));
                if (comp == null) return;

                // 2. 获取方法
                MethodInfo addMethod = RimTalkReflectionCache.AddActiveMemoryMethod ?? RimTalkReflectionCache.AddMemoryMethod;
                if (addMethod == null) return;

                // 3. 准备内容
                string content = "CulinaryArts.AteDish".CanTranslate()
                    ? "CulinaryArts.AteDish".Translate(dishName).ToString()
                    : $"享用了 {dishName}";

                // 4. 调用
                addMethod.Invoke(comp, new object[] { content, RimTalkReflectionCache.ActionType, 2.0f, null });
            }
            catch (Exception)
            {
                // 保持静默
            }
        }

        /// <summary>
        /// RimTalk 兼容性桥接方法 - 删除原版食物记忆
        /// </summary>
        private static void RemoveRimTalkVanillaFoodMemory(Pawn pawn)
        {
            try
            {
                RimTalkReflectionCache.EnsureInitialized();
                if (!RimTalkReflectionCache.IsRimTalkActive) return;
                
                // 检查必要的反射字段是否存在
                if (RimTalkReflectionCache.ActiveMemoriesProp == null || 
                    RimTalkReflectionCache.TimestampField == null || 
                    RimTalkReflectionCache.TypeField == null || 
                    RimTalkReflectionCache.ContentField == null) return;

                // 1. 查找组件
                Type compType = RimTalkReflectionCache.FourLayerMemoryCompType;
                if (compType == null) return;

                var comp = pawn.AllComps.Find(c => compType.IsAssignableFrom(c.GetType()));
                if (comp == null) return;

                // 2. 获取 ActiveMemories 列表
                var activeMemories = RimTalkReflectionCache.ActiveMemoriesProp.GetValue(comp) as System.Collections.IList;
                if (activeMemories == null || activeMemories.Count == 0) return;

                // 3. 查找并删除匹配的记忆
                int currentTick = Find.TickManager.TicksGame;
                var toRemoveList = new System.Collections.Generic.List<object>();

                // 优化：从后往前遍历（最新的记忆在最后）
                for (int i = activeMemories.Count - 1; i >= 0; i--)
                {
                    var memory = activeMemories[i];
                    
                    // 使用缓存的 FieldInfo 获取时间戳
                    var timestamp = (int)RimTalkReflectionCache.TimestampField.GetValue(memory);
                    int age = currentTick - timestamp;
                    
                    // 缩小时间窗口到 900 ticks (约15秒)，只检查最近的记忆
                    if (age > 900) break; 
                    
                    // 检查类型
                    var type = RimTalkReflectionCache.TypeField.GetValue(memory);
                    if (!type.Equals(RimTalkReflectionCache.ActionType)) continue;

                    // 检查内容
                    var content = RimTalkReflectionCache.ContentField.GetValue(memory) as string;
                    if (string.IsNullOrEmpty(content)) continue;

                    // 优化：只检查最常见的关键词
                    if (content.Contains("吃了") || content.Contains("享用") || content.Contains("食用") ||
                        content.Contains("ate ") || content.Contains("Ate ") || 
                        content.Contains("简单餐") || content.Contains("精致餐") || content.Contains("奢侈餐") ||
                        content.Contains("MealSimple") || content.Contains("MealFine") || content.Contains("MealLavish"))
                    {
                        toRemoveList.Add(memory);
                    }
                }

                // 4. 删除找到的所有记忆
                foreach (var memory in toRemoveList)
                {
                    activeMemories.Remove(memory);
                    if (CulinaryArtsSettings.ShowDebugLog)
                    {
                        var content = RimTalkReflectionCache.ContentField.GetValue(memory) as string;
                        Log.Message($"[厨间百艺] 已删除 RimTalk 原版食物记忆: {content}");
                    }
                }
            }
            catch (Exception)
            {
                // 保持静默
            }
        }
    }
}
