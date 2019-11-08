using System;

namespace SemestreProject.Snake
{
    public class GameObject : BaseObject
    {
        protected int posX { get; set; }
        protected int posY { get; set; }

        public int GetPositionX()
        {
            return posX;
        }

        public int GetPositionY()
        {
            return posY;
        }

        public void SetPositionX(int x)
        {
            posX = x;
        }

        public void SetPositionY(int y)
        {
            posY = y;
        }

    }
}