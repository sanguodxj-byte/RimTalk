using System;
using Verse;

namespace Sideria.BloodThornKnight
{
    [StaticConstructorOnStartup]
    public class SideriaMod : Mod
    {
        public SideriaMod(ModContentPack content) : base(content)
        {
            Log.Message("[Sideria] BloodThorn Knight mod loaded successfully!");
        }
    }
}
