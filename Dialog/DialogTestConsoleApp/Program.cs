using DialogLibrary.App.DialogSystem.Datasets;
using YuiGameSystems.DialogSystem;
using YuiGameSystems.DialogSystem.FileLoading.DataFiles;

DatasetLocations datasetLocations = new() {
	dialogs = "../../../ExampleData/Dialog/json/Dialogs",
	characters = "../../../ExampleData/Dialog/json/Characters",
	events = "../../../ExampleData/Dialog/json/Events",
	traits = "../../../ExampleData/Dialog/json/Traits"
};

DatasetManager datasetManager = new();
datasetManager.LoadDtoDataIntoMemory(datasetLocations);

datasetManager.Datasets.Events.TryGetValue("battle_event", out Event? battleEvent);
if (battleEvent is null) throw new Exception("Battle event not found");
battleEvent.Action = () => Console.WriteLine("Battle event triggered");

datasetManager.Datasets.Events.TryGetValue("stop_interaction_event", out Event? stopInteractionEvent);
if (stopInteractionEvent is null) throw new Exception("Stop interaction event not found");
stopInteractionEvent.Action = () => Console.WriteLine("Stop interaction event triggered");

void DisplayNpcPromptRequested(string prompt) {
    Console.WriteLine(prompt);
}

string PlayerAnswerRequested() {
	Console.Write("Answer: ");
	return Console.ReadLine() ?? throw new Exception("Answer cannot be null");
}

void DisplayPlayerPromptChoicesRequested(string[] choices) {
	Console.WriteLine("Choices: ");
	foreach (string choice in choices) {
		Console.WriteLine(choice);
	}
}

DialogActions dialogActions = new(
	() => { },
	() => { },
	DisplayNpcPromptRequested,
	DisplayPlayerPromptChoicesRequested,
	PlayerAnswerRequested
);

while (true) {
	Dialog? dialog = null;
	Npc? player = null;
	Npc? Interlocutor = null;

	datasetManager.Datasets.Characters.TryGetValue("Yui", out Npc? targetNpc);
    if (targetNpc is null) throw new Exception("Yui not found");

	Console.WriteLine("Is this a player conversation?");
    Console.WriteLine("1. Yes");
    Console.WriteLine("2. No");
    Console.Write("Answer: ");

    string? answer = Console.ReadLine() ?? throw new Exception("Answer is null");
    if (answer == "1")
    {
        Console.WriteLine("Which traits does the player have? use a space to add multiple");

        List<string> traits = [.. datasetManager.Datasets.Traits.Keys];
        for (int i = 0; i < traits.Count; i++)
        {
            Console.ForegroundColor = (ConsoleColor)((i % 15) + 1);
            Console.Write(traits[i]);

            if (i % 5 == 0 && i != 0)
            {
                Console.WriteLine();
            } else
            {
                Console.Write(" ");
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;

        Console.Write("Answer: ");
        answer = Console.ReadLine() ?? throw new Exception("Answer is null");

        // if no answer is given, we just use the default player
        List<Trait?> playerTraits = [];
        if (answer != string.Empty)
        {
            playerTraits = answer.Split(' ').Select(t => datasetManager.Datasets.Traits.TryGetValue(t, out Trait? trait) ? trait : null).ToList();
        }

        List<Trait> traitsWithNoNull = playerTraits.Where(t => t is not null).Select(t => t!).ToList();
        player = new Npc("Player", "Player", traitsWithNoNull.ConvertAll(t => t.Name), []);
    } else
    {
        if (answer != "2")
            return;
    }

    Console.Clear();

	if (player is not null) {
        dialog = new Dialog(datasetManager, "example", targetNpc, player, true, dialogActions);
	} else {
        //DialogDatasets.Npcs.TryGetValue("Altaira", out var inter);
        //Interlocutor = inter;traitsWithNoNull
        Interlocutor = datasetManager.Datasets.Characters.Values.Where(n => n != targetNpc).ToList()[new Random().Next(0, datasetManager.Datasets.Characters.Count - 1)];
        Console.WriteLine($"Target: {targetNpc.Name}");
        Console.WriteLine($"Interlocutor: {Interlocutor.Name}");
        dialog = new Dialog(datasetManager, "example", targetNpc, Interlocutor, false, dialogActions);
	}

    Console.ReadKey();
    Console.Clear();
}
