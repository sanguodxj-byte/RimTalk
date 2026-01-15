using Verse;

namespace ListenToMe
{
    public static class LoadedModManager_Patch
    {
        [StaticConstructorOnStartup]
        public static class Startup
        {
            static Startup()
            {
                Log.Message("[ListenToMe] Listen To Me mod loaded successfully!");
                Log.Message("[ListenToMe] Version: 1.0.0");
                Log.Message("[ListenToMe] Features:");
                Log.Message("[ListenToMe] - Text command parsing (keyword + AI analysis)");
                Log.Message("[ListenToMe] - RimTalk dialogue system");
                Log.Message("[ListenToMe] - Workbench task recognition");
                Log.Message("[ListenToMe] - Auto-create crafting bills");
            }
        }
    }
}
