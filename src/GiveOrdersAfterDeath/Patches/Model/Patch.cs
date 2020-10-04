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
      this._harmony = harmony;
    }

    public void Apply()
    {
      if (this.Applied)
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
            this._harmony.Patch((MethodBase) patchedMethod, harmonyMethod, (HarmonyMethod) null, (HarmonyMethod) null, (HarmonyMethod) null);
            break;
          case HarmonyPatchType.Postfix:
            this._harmony.Patch((MethodBase) patchedMethod, (HarmonyMethod) null, harmonyMethod, (HarmonyMethod) null, (HarmonyMethod) null);
            break;
          case HarmonyPatchType.Transpiler:
            this._harmony.Patch((MethodBase) patchedMethod, (HarmonyMethod) null, (HarmonyMethod) null, harmonyMethod, (HarmonyMethod) null);
            break;
          case HarmonyPatchType.Finalizer:
            this._harmony.Patch((MethodBase) patchedMethod, (HarmonyMethod) null, (HarmonyMethod) null, (HarmonyMethod) null, harmonyMethod);
            break;
          default:
            throw new NotSupportedException(string.Format("{0} action is not supported when manually patching", (object) harmonyPatchType));
        }
      }
      this.Applied = true;
    }

    public void Reset()
    {
      if (!this.Applied)
        return;
      foreach (PatchMethodInfo patchMethodInfo in this.GetPatchMethodsInfo())
        this._harmony.Unpatch((MethodBase) patchMethodInfo.PatchedMethod, patchMethodInfo.PatchMethod);
      this.Applied = false;
    }

    public bool Applied { get; private set; }

    public abstract IEnumerable<PatchMethodInfo> GetPatchMethodsInfo();
  }
}
