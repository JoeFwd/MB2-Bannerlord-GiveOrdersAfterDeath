using System;
using System.Collections.Generic;
using System.Reflection;

namespace GiveOrdersAfterDeath
{
    public partial class GiveOrdersAfterDeathSubModule
    {
        private static List<IPatch> _patches;

        private static List<IPatch> Patches {
            get {
                if (_patches != null)
                    return _patches;

                var patchInterfaceType = typeof(IPatch);
                _patches = new List<IPatch>();

                foreach (var type in typeof(GiveOrdersAfterDeathSubModule).Assembly.GetExportedTypes()) {
                    if (type.IsInterface || type.IsAbstract)
                        continue;
                    if (!patchInterfaceType.IsAssignableFrom(type))
                        continue;

                    try {
                        var patch = (IPatch) Activator.CreateInstance(type, true);
                        _patches.Add(patch);
                    }
                    catch (TargetInvocationException tie)
                    {
                        Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
                    }
                    catch (Exception ex) {
                        Error(ex, $"Failed to create instance of patch: {type.FullName}");
                    }
                }

                return _patches;
            }
        }

        private static void ApplyPatches()
            => Patches.ForEach(patch =>
            {
                try {
                    patch.Apply();
                    Print($"{(patch.Applied ? "Applied" : "Skipped")} Patch: {patch.GetType()}");
                }
                catch (Exception ex) {
                    Error(ex, $"Error while applying patch: {patch.GetType().Name}");
                }
            });
        
        private static void ResetPatches()
            => Patches.ForEach(patch => 
            {
                try {
                    patch.Reset();
                }
                catch (Exception ex) {
                    Error(ex, $"Error while resetting patch: {patch.GetType().Name}");
                }
            });

        private static bool ArePatchesApplied()
            => !Patches.Exists(patch => !patch.Applied);

    }
}