using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public static class AcquaintanceRepo {
	public static Acquaintance? GetAcquaintanceOrNullIfTalkingToPlayer(string currentNpcId, Npc dialogNpc, Npc interNpc, bool isPlayerConvo) {
		Acquaintance? Acquaintance = null;

		// if the DialogNpc is speaking
		if (currentNpcId == dialogNpc.Id) {
			if (!isPlayerConvo) // A player does not have a attitude towards npc's
                Acquaintance = GetAcquaintance(dialogNpc, interNpc);
		}
		else if (currentNpcId == interNpc.Id) { // if the player or a interNpc said something
			Acquaintance = GetAcquaintance(interNpc, dialogNpc); // get the dialogNpc since we will alter its attitude
        }
		else {
			throw new Exception("Something went wrong trying to get a acquaintance!");
		}

		return Acquaintance;
	}

	public static void AddAsAcquaintances(Npc npc, Npc inter) {
        AddAsAcquaintance(npc, inter);
        AddAsAcquaintance(inter, npc);
	}

	private static Acquaintance GetAcquaintance(Npc forNpc, Npc findNpc) {
		forNpc.Acquaintances ??= [];

		Acquaintance? acquaintance = forNpc.Acquaintances.Find(x => x.Id == findNpc.Id);
		return acquaintance ?? AddAsAcquaintance(forNpc, findNpc);
	}

	private static Acquaintance AddAsAcquaintance(Npc toNpc, Npc addNpc) {
		toNpc.Acquaintances ??= [];

        Acquaintance acquaintance = new(addNpc.Id, 0);
		if (!toNpc.Acquaintances.Any(x => x.Id == addNpc.Id))
			toNpc.Acquaintances.Add(acquaintance);

		return acquaintance;
	}
}
