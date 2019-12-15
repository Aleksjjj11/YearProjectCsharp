using System;

namespace SemestreProject.Snake
{
    public class Fruit : GameObject
    {
        private Random _random;
        public Fruit()
        {
            _random = new Random();
        }
        
        public void NewPosition(GameField gameField)
        {
            int posX = _random.Next(0, gameField.GetWidth() - 1);
            int posY = _random.Next(0, gameField.GetHeight() - 1);

            cell = gameField.GetCell(posX, posY);
        }

        public override char GetSymbol()
        {
            return '$';
        }
    }
}