using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GiveOrdersAfterDeath.Patches.Model
{
  public abstract class Patch<TPatch> : IPatch where TPatch : IPatch
  {
    private readonly Harmony _harmony;

    protected Patch(Harmony harmony)
    {
      _harmony = harmony;
    }

    public void Apply()
    {
      if (Applied)
        return;
      foreach (PatchMethodInfo patchMethodInfo in this.GetPatchMethodsInfo())
      {
        MethodInfo patchMethod = patchMethodInfo.PatchMethod;
        MethodInfo patchedMethod = patchMethodInfo.PatchedMethod;
        HarmonyPatchType harmonyPatchType = patchMethodInfo.HarmonyPatchType;
        HarmonyMethod harmonyMethod = new HarmonyMethod(patchMethod);
        switch (harmonyPatchType)
        {
          case HarmonyPatchType.Prefix:
            _harmony.Patch(patchedMethod, prefix: harmonyMethod);
            break;
          case HarmonyPatchType.Postfix:
            _harmony.Patch(patchedMethod, postfix: harmonyMethod);
            break;
          case HarmonyPatchType.Transpiler:
            _harmony.Patch(patchedMethod, transpiler: harmonyMethod);
            break;
          case HarmonyPatchType.Finalizer:
            _harmony.Patch(patchedMethod, finalizer: harmonyMethod);
            break;
          default:
            throw new NotSupportedException($"{harmonyPatchType} action is not supported when manually patching");
        }
      }
      Applied = true;
    }

    public void Reset()
    {
      if (!Applied)
        return;

      foreach (PatchMethodInfo patchMethodInfo in this.GetPatchMethodsInfo())
      {
        _harmony.Unpatch(patchMethodInfo.PatchedMethod, patchMethodInfo.PatchMethod);
      }
        
      Applied = false;
    }

    public bool Applied { get; private set; }

    public abstract IEnumerable<PatchMethodInfo> GetPatchMethodsInfo();
  }
}
