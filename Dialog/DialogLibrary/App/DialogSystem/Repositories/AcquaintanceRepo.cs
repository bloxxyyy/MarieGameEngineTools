using DialogLibrary.App.Helpers;

using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public static class AcquaintanceRepo {
    public static bool DoesNpcHaveAcquaintance(Npc forNpc, Npc findNpc) {
        return forNpc.Acquaintances?.Any(x => x.Id == findNpc.Id) ?? false;
    }

    public static Acquaintance? TryGetAcquaintance(Npc forNpc, Npc findNpc) {
        return forNpc.Acquaintances?.FirstOrDefault(a => a.Id == findNpc.Id);
    }

    public static void TryAddAsAcquaintance(Npc forNpc, Npc findNpc) {
        if (!DoesNpcHaveAcquaintance(forNpc, findNpc)) {
            forNpc.Acquaintances = Guard.ListOrNullToList(forNpc.Acquaintances);
            forNpc.Acquaintances.Add(new(findNpc.Id, 0));
        }
    }
}
