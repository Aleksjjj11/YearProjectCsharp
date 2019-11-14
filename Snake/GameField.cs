using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SemestreProject.Snake
{
    public class GameField
    {
        private int _height;
        private int _width;
        private int _score;
        private Cell[,] fields;
        public GameField(int heightInput = 10, int widthInput = 15)
        {
            _score = 0;
            _height = heightInput < 10 ? 10 : heightInput;
            _width = widthInput < 15 ? 15 : widthInput;
            fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    fields[i,j] = new Cell(i,j);
        }

        public Cell GetCell(int x, int y)
        {
            return fields[y, x];
        }
        public void SetHeight(int value)
        {
            _height = value;
            fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    fields[i,j] = new Cell(i,j);
        }
        public void SetWidth(int value)
        {
            _width = value;
            fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    fields[i,j] = new Cell(i,j);
        }
        public void SetField(Cell[,] field)
        {
            fields = field;
        }

        public void SetScore(int value)
        {
            _score = value;
        }

        public int GetScore()
        {
            return _score;
        }
        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

        private void SaveInFile()
        {
            Console.Write("Введите имя для файла сохранения: ");
            string fileName = Console.ReadLine();
            StreamWriter file = new StreamWriter($"{fileName}.snk");
            file.WriteLine(_height + "/" + _width);
            file.WriteLine(_score);
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
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
            string line = null;
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
            _height = fileHeight;
            _width = fileWidth;
            Console.WriteLine($"height = {_height} width = {_width}");
            Cell[,] fileField = new Cell[_height,_width];
            int i, j;
            for (i = 0; i < _height; i++)
                for (j = 0; j < _width; j++)
                    fileField[i,j] = new Cell(i,j);
            
            snake.GetTail().Clear();
            //Upload score from file
            int fileScore = Convert.ToInt32(file.ReadLine());
            _score = fileScore;
            int numHead = 0, numFruit = 0;
            i = 0;
            int obj;
            while (i < _height)
            {
                j = 0;
                
                while (j < _width)
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
                            snake.GetHead().SetCell(fileField[i,j]);
                            fileField[i, j].obj = snake.GetHead();
                            numHead++;
                            break;
                        }
                        case 2: //Tail
                        {
                            snake.GetTail().Add(new Tail(fileField[i,j]));
                            fileField[i, j].obj = snake.GetTail().Last();
                            break;
                        }
                        case 3: //Fruit
                        {
                            fruit.SetCell(fileField[i,j]);
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
            String oldBuffer = " ";
            String newBuffer = "";
            Console.Clear();
            newBuffer += $"\nYour score {_score}\n";
            for (int i = 0; i < _width + 1; i++)
            {
                newBuffer += "#";
            }
            newBuffer += "\t\tEsp - Pause\n";
            for (int i = 0; i < _height; i++)
            {
                newBuffer += "#"; 
                for (int j = 0; j < _width; j++)
                {
                    if (fields[i, j].obj == null)
                    {
                        newBuffer += " ";
                        continue;
                        
                    }
                    if (fields[i, j].obj.GetType().Name == "Fruit")
                    {
                        newBuffer += "$";
                        continue;
                    }
                    if (fields[i, j].obj.GetType().Name == "Head")
                    {
                        newBuffer += "@";
                        continue;
                    }
                    if (fields[i, j].obj.GetType().Name == "Tail")
                    {
                        newBuffer += "*";
                        continue;
                    }
                }
                newBuffer += "#\n";
            }

            for (int i = 0; i < _width + 1; i++)
            {
                newBuffer += "#";
            }

            oldBuffer = oldBuffer.Replace(oldBuffer, newBuffer);
            Console.Write(oldBuffer);
        }

        public bool IsWin()
        {
            if (_score >= _height * _width * 10) return true;
            return false;
        }

        public void AddGameObject(GameObject gameObject)
        {
            Console.WriteLine(gameObject.GetType().Name);
            fields[gameObject.GetCell().Y, gameObject.GetCell().X].obj = gameObject;
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
                        snake.Reset();
                        fields = new Cell[_height, _width];
                        for (int i = 0; i < _height; i++)
                            for (int j = 0; j < _width; j++)
                                fields[i,j] = new Cell(i,j);
                        fruit.NewPosition(this);
                        AddGameObject(fruit);
                        AddSnake(snake);
                        _score = 0;
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

            snake.GetHead().GetCell().obj = null;
            if (snake.GetTail().Count != 0)
            {
                snake.GetTail().Last().GetCell().obj = null;
            }
            snake.Move(this);
            AddSnake(snake);
            if (snake.IsFruit(fruit))
            {
                _score += 10;
                if (snake.speed > 50) snake.UpSpeed();
                snake.GetTail().Add(snake.GetTail().Count is 0
                    ? new Tail(snake.GetHead().GetCell())
                    : new Tail(snake.GetTail().Last().GetCell()));
                do
                {
                    fruit.NewPosition(this);
                } while (fruit.GetCell().obj != null);
                AddGameObject(fruit);
            }
            return false;
        }
    }
}