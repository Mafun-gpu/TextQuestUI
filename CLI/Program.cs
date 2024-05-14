using UniExamQuest;

namespace CLI;

class Program
{

    static private int AskInput(string question)
    {
        Console.Write(question);
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, введите цифру.");
        }
        return choice;
    }

    static void Main(string[] args)
    {
        Game currentGame = new Game();
        GameMananger currentGameManager = new GameMananger();

        while (true)
        {
            Console.Clear();

            switch (currentGame.state)
            {
                case CLIState.Start:
                    Console.WriteLine("Добро пожаловать в игру");
                    Console.WriteLine("1. Начать новую игру");
                    Console.WriteLine("2. Загрузить сохранение");

                    currentGame.ProcessInput(AskInput(">> "));

                    break;
                case CLIState.NewGame:
                    Console.WriteLine("Как тебя зовут?");

                    var name = Console.ReadLine();
                    currentGameManager.NewGame(name == null ? "NO NAME" : name);

                    currentGame.ProcessInput(null);

                    break;
                case CLIState.LoadGame:
                    string[] files = Directory.GetFiles("./GameSaves");
                    if (files.Length > 0)
                    {
                        Console.WriteLine("Какое сохранение загрузить?");
                        foreach (string file in files)
                            Console.WriteLine(file);

                        Console.WriteLine("");
                        Console.Write("[Загрузить игру]>[Введите название сохранения]>");
                        var saveName = Console.ReadLine();
                        currentGameManager.LoadGame(saveName);
                        currentGame.ProcessInput(1);
                    }
                    else
                    {
                        Console.WriteLine("Нет сохранений :(");
                        Console.ReadLine();
                        currentGame.ProcessInput(2);
                    }


                    break;
                case CLIState.GameProfilePage:
                    if (currentGameManager.State.Player.Health == 0)
                    {
                        Console.WriteLine("ВЫ УМЕРЛИ");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    Console.WriteLine($"Игрок: {currentGameManager.State.Player.Name}");
                    var level = currentGameManager.State.Player.Mind / currentGameManager.State.Settings.MindToUpLevel;
                    Console.WriteLine($"Уровень: {level}");
                    Console.WriteLine($"День: {currentGameManager.State.Day}");
                    Console.WriteLine($"-------");
                    Console.WriteLine($"Здоровье: {currentGameManager.State.Player.Health}");
                    Console.WriteLine($"Еда: {currentGameManager.State.Player.Satiation}");
                    Console.WriteLine($"Счастье: {currentGameManager.State.Player.Happiness}");
                    Console.WriteLine($"-------");
                    Console.WriteLine($"Баланс: {currentGameManager.State.Player.Money}");

                    var exp = currentGameManager.State.Player.Mind % currentGameManager.State.Settings.MindToUpLevel;
                    Console.WriteLine($"Знания: {exp} / {currentGameManager.State.Settings.MindToUpLevel}");

                    Console.WriteLine($"");

                    Console.WriteLine($"Инвентарь");
                    foreach (var item in currentGameManager.State.Player.Inventory.Content)
                        Console.WriteLine($"    {item.Key.Name} x{item.Value}");

                    Console.WriteLine($"\n");
                    Console.WriteLine($"-------");

                    Console.WriteLine($"1. Инвентарь");
                    Console.WriteLine($"2. Задания");
                    Console.WriteLine($"3. Магазин");
                    Console.WriteLine($"4. Поспать");

                    Console.WriteLine($"0. Выйти");

                    var input = AskInput(">> ");
                    if (input == 4)
                        currentGameManager.State.NextDay();

                    currentGame.ProcessInput(input);

                    break;
                case CLIState.GameInventoryPage:
                    Console.WriteLine($"Инвентарь");
                    Console.WriteLine($"-------");

                    int i = 1;
                    var inventoryContent = currentGameManager.State.Player.Inventory.Content.ToArray();
                    foreach (var item in inventoryContent)
                        Console.WriteLine($"{i++}.{item.Key.Name}\tx{item.Value}");

                    Console.WriteLine($"\n");

                    Console.WriteLine($"Что будем использовать? (0. Назад)");
                    var inventoryChoice = AskInput("[Инвентарь]>> ");

                    if (inventoryChoice == 0)
                        currentGame.ProcessInput(null);
                    else if (inventoryChoice < i)
                    {
                        Console.Clear();
                        Console.WriteLine($"[{inventoryContent[inventoryChoice - 1].Key.Name}]");
                        Console.WriteLine($"Эффект:");
                        Console.WriteLine($"Еда {inventoryContent[inventoryChoice - 1].Key.Satiation}");
                        Console.WriteLine($"Счастье {inventoryContent[inventoryChoice - 1].Key.Happiness}");
                        Console.WriteLine($"Здоровье {inventoryContent[inventoryChoice - 1].Key.Health}");
                        Console.WriteLine($"Знания {inventoryContent[inventoryChoice - 1].Key.Mind}");

                        Console.WriteLine($"\n");
                        Console.WriteLine($"Используем?");
                        Console.WriteLine($"1. Да");
                        Console.WriteLine($"2. Нет");
                        var choice = AskInput($"[Инвентарь]>[{inventoryContent[inventoryChoice - 1].Key.Name}]> ");
                        if (choice == 1)
                        {
                            currentGameManager.State.Player.InteractWith(inventoryContent[inventoryChoice - 1].Key);
                            currentGameManager.State.Player.Inventory.Remove(inventoryContent[inventoryChoice - 1].Key);
                            continue;
                        }
                        else if (choice == 2)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Такого варианта нет :(");
                        Console.ReadLine();
                    }

                    break;
                case CLIState.GameStorePage:
                    Console.WriteLine($"Магазин");
                    Console.WriteLine($"-------");

                    var storeItems = currentGameManager.State.Store.GetItems();
                    int ii = 1;
                    foreach (var item in storeItems)
                        Console.WriteLine($"{ii++}.{item.Name} - {item.Price}руб.");

                    Console.WriteLine($"\n");

                    Console.WriteLine($"Что будем покупать? (0. Назад)");
                    var itemChoice = AskInput("[Магазин]>> ");

                    if (itemChoice == 0)
                        currentGame.ProcessInput(null);
                    else if (itemChoice < ii)
                    {
                        Console.Clear();
                        Console.WriteLine($"[{storeItems[itemChoice - 1].Name}]");
                        Console.WriteLine($"Цена: {storeItems[itemChoice - 1].Price}руб.");
                        Console.WriteLine($"Эффект:");
                        Console.WriteLine($"Счастье {storeItems[itemChoice - 1].Happiness}");
                        Console.WriteLine($"Знания {storeItems[itemChoice - 1].Mind}");
                        Console.WriteLine($"Здоровье {storeItems[itemChoice - 1].Health}");
                        Console.WriteLine($"Еда {storeItems[itemChoice - 1].Satiation}");

                        Console.WriteLine($"\n");
                        Console.WriteLine($"Покупаем?");
                        Console.WriteLine($"1. Да");
                        Console.WriteLine($"2. Нет");
                        var choice = AskInput($"[Магазин]>[{storeItems[itemChoice - 1].Name}]> ");
                        if (choice == 1)
                        {
                            if (currentGameManager.State.Player.Money >= storeItems[itemChoice - 1].Price)
                            {
                                currentGameManager.State.Player.Money -= storeItems[itemChoice - 1].Price;
                                currentGameManager.State.Player.Inventory.Add(storeItems[itemChoice - 1]);
                            }
                            else
                            {
                                Console.WriteLine("Недостаточно денег");
                                Console.ReadLine();
                            }
                            continue;
                        }
                        else if (choice == 2)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Такого варианта нет :(");
                        Console.ReadLine();
                    }

                    break;
                case CLIState.GameActivityPage:
                    Console.WriteLine($"Задания");
                    Console.WriteLine($"-------");
                    var questItems = currentGameManager.State.Quests;
                    ii = 1;
                    foreach (var item in questItems)
                        Console.WriteLine($"{ii++}. [{item.Type}] {item.Name} - {item.Money}руб.");

                    Console.WriteLine($"\n");

                    Console.WriteLine($"Чем будем заниматься? (0. Назад)");
                    var questChoice = AskInput("[Задания]>> ");

                    if (questChoice == 0)
                        currentGame.ProcessInput(null);
                    else if (questChoice < ii)
                    {
                        Console.Clear();
                        Console.WriteLine($"[{questItems[questChoice - 1].Name}]");
                        Console.WriteLine($"Прибыль: {questItems[questChoice - 1].Money}руб.");
                        Console.WriteLine($"Эффект:");
                        Console.WriteLine($"Счастье {questItems[questChoice - 1].Happiness}");
                        Console.WriteLine($"Знания {questItems[questChoice - 1].Mind}");
                        Console.WriteLine($"Здоровье {questItems[questChoice - 1].Health}");
                        Console.WriteLine($"Еда {questItems[questChoice - 1].Satiation}");

                        Console.WriteLine($"\n");
                        Console.WriteLine($"Выполняем?");
                        Console.WriteLine($"1. Да");
                        Console.WriteLine($"2. Нет");
                        var choice = AskInput($"[Задания]>[{questItems[questChoice - 1].Name}]> ");
                        if (choice == 1)
                        {
                            currentGameManager.State.Player.InteractWith(questItems[questChoice - 1]);
                            continue;
                        }
                        else if (choice == 2)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Такого варианта нет :(");
                        Console.ReadLine();
                    }

                    break;
                case CLIState.Exit:
                    Console.WriteLine($"Сохранить игру?");
                    Console.WriteLine($"-------");

                    Console.WriteLine($"1. Да");
                    Console.WriteLine($"2. Нет");

                    var saveGame = AskInput("[Сохранить]>> ");
                    if (saveGame == 1)
                    {
                        Console.Clear();
                        Console.WriteLine($"Сохранить игру?");
                        Console.WriteLine($"-------");
                        Console.Write("[Сохранить]>[Название сохранения]> ");

                        string? saveName = null;
                        while (saveName is null)
                            saveName = Console.ReadLine();

                        currentGameManager.SaveGame(saveName);
                    }

                    currentGame.ProcessInput(saveGame);

                    break;
                default:
                    break;
            }

        }
    }
}
