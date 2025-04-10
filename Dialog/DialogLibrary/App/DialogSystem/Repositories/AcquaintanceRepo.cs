using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

namespace DialogLibrary.App.DialogSystem.Repositories;
public class AcquaintanceRepo {

	public Acquaintance? GetAcquaintanceOrNullIfTalkingToPlayer(string currentNpcId, Npc dialogNpc, Npc interNpc, bool isPlayerConvo) {
		Acquaintance? Acquaintance = null;

		// if the DialogNpc is speaking
		if (currentNpcId == dialogNpc.Id) {

			// A player does not have a attitude towards npc's
			if (!isPlayerConvo)
				Acquaintance = GetAcquaintance(dialogNpc, interNpc);

		} else if (currentNpcId == interNpc.Id) { // if the player or a interNpc said something

			// get the dialogNpc since we will alter its attitude
			Acquaintance = GetAcquaintance(interNpc, dialogNpc);
		} else {
			throw new Exception("Something went wrong trying to get a acquaintance!");
		}

		return Acquaintance;
	}

	public void AddAsAcquaintances(Npc npc, Npc inter) {
		AddAsAcquaintance(npc, inter);
		AddAsAcquaintance(inter, npc);
	}

	private Acquaintance GetAcquaintance(Npc forNpc, Npc findNpc) {
		forNpc.Acquaintances ??= [];

		Acquaintance? acquaintance = forNpc.Acquaintances.Find(x => x.Id == findNpc.Id);
		return acquaintance ?? AddAsAcquaintance(forNpc, findNpc);
	}

	private Acquaintance AddAsAcquaintance(Npc toNpc, Npc addNpc) {
		toNpc.Acquaintances ??= [];

		var acquaintance = new Acquaintance(addNpc.Id, 0);
		if (!toNpc.Acquaintances.Any(x => x.Id == addNpc.Id))
			toNpc.Acquaintances.Add(acquaintance);

		return acquaintance;
	}
}
