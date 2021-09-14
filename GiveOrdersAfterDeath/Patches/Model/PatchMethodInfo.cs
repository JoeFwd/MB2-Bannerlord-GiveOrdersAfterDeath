using System.Reflection;
using HarmonyLib;

namespace GiveOrdersAfterDeath.Patches.Model
{
    public class PatchMethodInfo : IPatchMethodInfo
    {
        public PatchMethodInfo(MethodInfo patchedMethod, HarmonyPatchType harmonyPatchType, MethodInfo patchMethod)
        {
            PatchedMethod = patchedMethod;
            HarmonyPatchType = harmonyPatchType;
            PatchMethod = patchMethod;
        }
        
        public MethodInfo PatchedMethod { get; }
        public HarmonyPatchType HarmonyPatchType { get; }
        public MethodInfo PatchMethod { get; }
    }
}