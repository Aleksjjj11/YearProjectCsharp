using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SemestreProject.Snake
{
    public class GameField
    {
        private int height;
        private int width;
        private int score;
        private Cell[,] fields;
        public GameField(int heightInput = 10, int widthInput = 15)
        {
            score = 0;
            height = heightInput < 10 ? 10 : heightInput;
            width = widthInput < 15 ? 15 : widthInput;
            fields = new Cell[height, width];
        }

        public void SetHeight(int value)
        {
            height = value;
            fields = new Cell[height, width];
        }
        public void SetWidth(int value)
        {
            width = value;
            fields = new Cell[height, width];
        }
        public void SetField(Cell[,] field)
        {
            fields = field;
        }

        public void SetScore(int value)
        {
            score = value;
        }

        public int GetScore()
        {
            return score;
        }
        public int GetHeight()
        {
            return height;
        }

        public int GetWidth()
        {
            return width;
        }

        public GameObject GetObjectsInTail(int x = 0, int y = 0)
        {
            return fields[y,x].obj;
        }

        public void SaveInFile()
        {
            Console.Write("Введите имя для файла сохранения: ");
            string fileName = Console.ReadLine();
            StreamWriter file = new StreamWriter($"{fileName}.snk");
            file.WriteLine(height + "/" + width);
            file.WriteLine(score);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (fields[i, j].obj == null)
                    {
                        file.Write(0);
                        continue;                        
                    }
                    if (fields[i, j].obj.GetType().Name == "Head")
                    {
                        file.Write(1);
                        continue;
                    }

                    if (fields[i, j].obj.GetType().Name == "Tail")
                    {
                        file.Write(2);
                        continue;
                    }

                    if (fields[i, j].obj.GetType().Name == "Fruit")
                    {
                        file.Write(3);
                    }
                }
                file.Write("\n");
            }
            file.Close();
        }
        public void Upload(Snake snake, Fruit fruit)
        {
            Console.Write("Введите имя для файла для загрузки: ");
            string fileName = Console.ReadLine();
            StreamReader file = new StreamReader($"{fileName}.snk");
            string line = "";
            line = file.ReadLine();
            if (line is null)
            {
                file.Close();
                return;
            }
            string[] lines = line.Split("/");
            if (lines.Length != 2) return;
            //Upload height and width from file
            int fileHeight = Convert.ToInt32(lines[0]), fileWidth = Convert.ToInt32(lines[1]);
            height = fileHeight;
            width = fileWidth;
            Console.WriteLine($"height = {height} width = {width}");
            Cell[,] fileField = new Cell[height,width];
            snake.GetTail().Clear();
            //Upload score from file
            int fileScore = Convert.ToInt32(file.ReadLine());
            score = fileScore;
            int numHead = 0, numFruit = 0;
            int i = 0, j, obj;
            while (i < height)
            {
                j = 0;
                
                while (j < width)
                {
                    obj = Convert.ToInt32(file.Read()) - 48;
                    switch (obj)
                    {
                        case 0:
                        {
                            fileField[i, j].obj = null;
                            break;
                        }
                        case 1: //Head
                        {
                            snake.GetHead().SetPositionX(j);
                            snake.GetHead().SetPositionY(i);
                            fileField[i, j].obj = snake.GetHead();
                            numHead++;
                            break;
                        }
                        case 2: //Tail
                        {
                            snake.GetTail().Add(new Tail(j,i));
                            fileField[i, j].obj = snake.GetTail().Last();
                            break;
                        }
                        case 3: //Fruit
                        {
                            fruit.SetPositionX(j);
                            fruit.SetPositionY(i);
                            fileField[i, j].obj = fruit;
                            numFruit++;
                            break;
                        }
                        case -38:
                        {
                            continue;
                        }
                        default:
                            continue;
                    }
                    j++;
                }
                i++;
            }
            if (numHead != 1 || numFruit != 1)
            {
                Console.WriteLine($"При загрузке карты произошла ошибка head = {numHead} fruit = {numFruit}");
                return;
            }
            
            fields = fileField;
            file.Close();
            Console.WriteLine("Uploaded");
            Console.ReadKey();
        }
        
        public void Render()
        {
            Console.Clear();
            Console.WriteLine($"Your score {score}");
            for (int i = 0; i < width+1; i++)
                Console.Write("#");
            Console.Write("\t\tEsp - Pause");
            Console.WriteLine();
            for (int i = 0; i < height; i++)
            {
                Console.Write("#");
                for (int j = 0; j < width; j++)
                {
                    if (fields[i, j].obj == null)
                    {
                        Console.Write((" "));
                        continue;
                        
                    }
                    if (fields[i, j].obj.GetType().Name == "Fruit")
                    {
                        Console.Write(("$"));
                        continue;
                    }
                    if (fields[i, j].obj.GetType().Name == "Head")
                    {
                        Console.Write(("@"));
                        continue;
                    }
                    if (fields[i, j].obj.GetType().Name == "Tail")
                    {
                        Console.Write(("*"));
                        continue;
                    }
                }
                Console.Write("#");
                Console.WriteLine();
            }
            for (int i = 0; i < width+1; i++)
                Console.Write("#");
        }

        public bool IsWin()
        {
            if (score >= height * width * 10) return true;
            return false;
        }

        public void AddGameObject(GameObject gameObject)
        {
            fields[gameObject.GetPositionY(), gameObject.GetPositionX()].obj = gameObject;
        }

        public void AddSnake(Snake snake)
        {
            AddGameObject(snake.GetHead());
            foreach (Tail tail in snake.GetTail())
            {
                AddGameObject(tail);
            }
        }

        public void ReadAction(Snake snake, Fruit fruit)
        {
            ConsoleKeyInfo key;
            while (Console.KeyAvailable)
                {
                    key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.A:
                        {
                            if (snake.vector is TypeVector.Right) return;
                            
                            snake.SwapVector(TypeVector.Left);
                            snake.moveStatus = MoveStatus.Moving;
                            break;
                        }
                        case ConsoleKey.W:
                        {
                            if (snake.vector is TypeVector.Down) return;

                            snake.SwapVector(TypeVector.Up);
                            snake.moveStatus = MoveStatus.Moving;
                            break;
                        }
                        case ConsoleKey.D:
                        {
                            if (snake.vector is TypeVector.Left) return;

                            snake.SwapVector(TypeVector.Right);
                            snake.moveStatus = MoveStatus.Moving;
                            break;
                        }
                        case ConsoleKey.S:
                        {
                            if (snake.vector is TypeVector.Up) return;

                            snake.SwapVector(TypeVector.Down);
                            snake.moveStatus = MoveStatus.Moving;
                            break;
                        }
                        case ConsoleKey.Escape:
                        {
                            snake.moveStatus = MoveStatus.Stopping;
                            break;
                        }
                        case ConsoleKey.R:
                        {
                            snake = new Snake();
                            fields = new Cell[height, width];
                            fruit.NewPosition(height, width);
                            AddGameObject(fruit);
                            score = 0;
                            Render();
                            break;
                        }
                        case ConsoleKey.F:
                        {
                            SaveInFile();
                            break;
                        }
                        case ConsoleKey.O:
                        {
                            Upload(snake, fruit);
                            Render();
                            break;
                        }
                        default: break;
                    }
                }
        }

        public bool Tick(Snake snake, Fruit fruit)
        {
            Thread.Sleep(snake.speed);
            if (snake.IsObstecle(this)) return true;

            fields[snake.GetHead().GetPositionY(), snake.GetHead().GetPositionX()].obj = null;
            if (snake.GetTail().Count != 0)
            {
                fields[snake.GetTail().Last().GetPositionY(), snake.GetTail().Last().GetPositionX()].obj = null;
            }
            snake.Move();
            AddSnake(snake);
            if (snake.IsFruit(fruit))
            {
                score += 10;
                if (snake.speed > 50) snake.UpSpeed();
                if (snake.GetTail().Count is 0)
                    snake.GetTail().Add(new Tail(snake.GetHead().GetPositionX(), snake.GetHead().GetPositionY()));
                else
                    snake.GetTail().Add(new Tail(snake.GetTail().Last().GetPositionX(), snake.GetTail().Last().GetPositionY()));
                do
                {
                    fruit.NewPosition(height, width);
                } while (GetObjectsInTail(fruit.GetPositionX(), fruit.GetPositionY()) != null);   
                AddGameObject(fruit);
            }

            return false;
        }
    }
}