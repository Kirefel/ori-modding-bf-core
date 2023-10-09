﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace kft.oribf.uilib.Map;

[HarmonyPatch(typeof(AreaMapIconManager), nameof(AreaMapIconManager.ShowAreaIcons))]
internal class AreaMapIconManagerCustomIconsPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            yield return instruction;

            if (instruction.opcode == OpCodes.Stloc_1)
            {
                yield return new CodeInstruction(OpCodes.Ldloc_1); // runtimeGameWorldArea
                yield return CodeInstruction.Call(typeof(AreaMapIconManagerCustomIconsPatch), "AddIcons"); // AddIcons(runtimeGameWorldArea)
            }
        }
    }

    private static void AddIcons(RuntimeGameWorldArea runtimeGameWorldArea)
    {
        foreach (var icon in CustomWorldMapIconManager.Icons)
        {
            if (!runtimeGameWorldArea.Area.InsideFace(icon.Position))
                continue;

            RuntimeWorldMapIcon runtimeWorldMapIcon = null;

            for (int i = runtimeGameWorldArea.Icons.Count - 1; i >= 0; i--)
            {
                if (runtimeGameWorldArea.Icons[i].Guid == icon.Guid)
                {
                    runtimeWorldMapIcon = runtimeGameWorldArea.Icons[i];
                    break;
                }
            }

            bool visible = icon.Visible?.Invoke(icon.Guid) ?? true;
            if (runtimeWorldMapIcon == null && visible)
            {
                var worldMapIcon = (GameWorldArea.WorldMapIcon)FormatterServices.GetUninitializedObject(typeof(GameWorldArea.WorldMapIcon));
                worldMapIcon.Guid = icon.Guid;
                worldMapIcon.IsSecret = icon.IsSecret;
                worldMapIcon.Position = icon.Position;

                runtimeGameWorldArea.Icons.Add(new RuntimeWorldMapIcon(worldMapIcon, runtimeGameWorldArea));
            }
            else if (runtimeWorldMapIcon != null)
            {
                runtimeWorldMapIcon.Position = icon.Position;
                runtimeWorldMapIcon.Icon = visible ? WorldMapIconType.EnergyGateTwelve : WorldMapIconType.Invisible;
            }
        }
    }
}
