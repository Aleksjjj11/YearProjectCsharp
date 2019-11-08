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
        
        public void NewPosition(int height, int width)
        {
            posX = _random.Next(0, width - 1);
            posY = _random.Next(0, height - 1);
        }
    }
}