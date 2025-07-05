workspace "MarieGameEngine" ", Sub deviced game engine project" {
	model {
        developer = person "Developer"
        engine = person "Engine"

        dialogSolution = softwareSystem "Dialog Solution" {
            dialogLibrary = container "Dialog library"
            dialogTestProject = container "Dialog Test Project"
            dialogConfigurator = container "Dialog Configurator"
        }

        developer -> dialogConfigurator "uses"
        engine -> dialogLibrary "uses"
        dialogTestProject -> dialogLibrary "tests"
        dialogConfigurator -> dialogLibrary "sends json to"

        marieBaseSolution = softwareSystem "Marie Base Solution" {
            marieBaseMigrationManager = container "Marie Base Migration Manager"
            marieBaseTestProject = container "Marie Base Test Project"
            marieBaseLibrary = container "Marie Base Library"
        }

        dialogConfigurator -> marieBaseLibrary "uses"

        database = softwareSystem "MongoDB"

        marieBaseTestProject -> marieBaseLibrary "Tests"
        marieBaseMigrationManager -> marieBaseLibrary "Calls database from"
        marieBaseLibrary -> database "Reads from and writes to"
        developer -> marieBaseMigrationManager "Migrates database using"
    }

    views {
        systemContext marieBaseSolution "Diagram1" {
            include *
            autolayout lr
        }

        container marieBaseSolution "Diagram2" {
            include *
            autolayout lr
        }

        container dialogSolution "Diagram3" {
            include *
            autolayout lr
        }
    }
}