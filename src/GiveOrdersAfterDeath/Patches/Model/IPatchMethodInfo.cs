using HarmonyLib;
using System.Reflection;

namespace GiveOrdersAfterDeath.Patches.Model
{
    public interface IPatchMethodInfo
    {
        MethodInfo PatchedMethod { get; }

        HarmonyPatchType HarmonyPatchType { get; }

        MethodInfo PatchMethod { get; }
    }
}