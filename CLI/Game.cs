namespace CLI;

enum CLIState
{
    Start,
    NewGame,
    LoadGame,
    GameProfilePage,
    GameInventoryPage,
    GameActivityPage,
    GameStorePage,
    Exit,
}

class Game
{
    public CLIState state { get; private set; }

    public Game()
    {
        this.state = CLIState.Start;
    }

    public void ProcessInput(int? choice)
    {
        switch (state)
        {
            case CLIState.Start:
                if (choice == 1)
                    state = CLIState.NewGame;
                else if (choice == 2)
                    state = CLIState.LoadGame;
                else
                    Console.WriteLine("Неверный вариант. Введите 1 или 2.");
                break;
            case CLIState.NewGame :
                state = CLIState.GameProfilePage;
                break;
            case CLIState.LoadGame:
                if (choice == 1)
                    state = CLIState.GameProfilePage;
                else if (choice == 2) 
                    state = CLIState.Start;
                break;
            case CLIState.GameProfilePage:
                if (choice == 1)
                    state = CLIState.GameInventoryPage;
                else if (choice == 2)
                    state = CLIState.GameActivityPage;
                else if (choice == 3)
                    state = CLIState.GameStorePage;
                else if (choice == 4)
                    state = CLIState.GameProfilePage;
                else if (choice == 0)
                    state = CLIState.Exit;
                else
                    Console.WriteLine("Неверный вариант. Введите 1, 2, 3 или 0.");
                break;
            case CLIState.GameInventoryPage or CLIState.GameStorePage or CLIState.GameActivityPage:
                state = CLIState.GameProfilePage;
                break;
            case CLIState.Exit:
                if (choice == 1)
                    state = CLIState.GameProfilePage;
                else if (choice == 2)
                    Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Unknown game state.");
                break;
        }
    }
}
