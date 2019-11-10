using System;
using System.IO;
using SemestreProject.Snake;

namespace SemestreProject
{
    class Game
    {
        static private string nameUser = "Anonymous";

        static void Main(string[] args)
        {
            Console.Write("Перед началом игры введите ваш ник: ");
            nameUser = Console.ReadLine();
            //For some reason need wrote Snake for correctly work.
            GameField gameField = new GameField();
            Snake.Snake snake = new Snake.Snake();
            Fruit fruit = new Fruit();
            Menu(snake, fruit, gameField);
        }

        static void Menu(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\t\t1) Новая игра\n" +
                              $"\t\t\t2) Продолжить игру\n" +
                              $"\t\t\t3) Просмотр всей таблицы рекордов\n" +
                              $"\t\t\t4) Просмотр своих рекордов\n" +
                              $"\t\t\t0) Выход\n");
            ConsoleKeyInfo key;
            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D0:
                {
                    Environment.Exit(0);
                    break;    
                }
                case ConsoleKey.D1:
                {
                    StartGame(snake, fruit, gameField);
                    Play(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D2:
                {
                    gameField.Upload(snake, fruit);
                    gameField.Render();
                    Play(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D3:
                {
                    ShowScoreTable(false);
                    Console.WriteLine("Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    Menu(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D4:
                {
                    ShowScoreTable(true);
                    Console.WriteLine("Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    Menu(snake, fruit, gameField);
                    break;
                }
                default:
                {
                    StartGame(snake, fruit, gameField);
                    break;
                }
            };
        }
        static public void Play(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            bool isLose = false;
            while (!gameField.IsWin() && !isLose)
            {
                //reading action
                gameField.ReadAction(snake, fruit);
                if (snake.moveStatus is MoveStatus.Moving)
                {
                    gameField.Render();
                    isLose = gameField.Tick(snake, fruit);
                    gameField.Render();
                }
            }
            SaveScore(gameField);
            if (isLose) 
                Console.WriteLine("Game over!\nYou lose.");
            else
                Console.WriteLine("Game finished! You won!! Incredible!!");
            Menu(snake, fruit, gameField);
        }
        static void ShowScoreTable(bool isPerson = false)
        {
            string fileName = "Score.txt";
            StreamReader fileReader = new StreamReader(fileName);
            int numLine = 0;
            Console.WriteLine("Ваши рекорды:");
            string line;
            while ((line = fileReader.ReadLine()) != null)
            {
                //If type of output is personal then will output only personal records
                //else all records
                if (isPerson)
                {
                    if (line.Split("-")[0] == nameUser)
                    {
                        Console.WriteLine($"{numLine+1}. {line}");
                        numLine++;
                    }    
                }
                else
                {
                    Console.WriteLine($"{numLine+1}. {line}");
                    numLine++;
                }
            }
            if (numLine is 0) Console.WriteLine("У вас ещё нет рекордов");
            fileReader.Close();
        }

        static void SaveScore(GameField gameField)
        {
            string fileName = "Score.txt";
            StreamWriter fileWriter = new StreamWriter(fileName, true);
            fileWriter.WriteLine(nameUser + "-" + gameField.GetScore());
            fileWriter.Close();
        }

        static void EnterSizeField(GameField gameField)
        {
            Console.WriteLine("Введите высоту поля");
            int height, width;
            height = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите ширину поля");
            width = Convert.ToInt32(Console.ReadLine());
            gameField.SetHeight(height);
            gameField.SetWidth(width);
        }

        static void StartGame(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            snake = new Snake.Snake();
            fruit = new Fruit();
            gameField = new GameField();
            EnterSizeField(gameField);
            fruit.NewPosition(gameField);
            gameField.AddSnake(snake);
            gameField.AddGameObject(fruit);
            
            gameField.Render();
            Play(snake, fruit, gameField);
        }
    }
}