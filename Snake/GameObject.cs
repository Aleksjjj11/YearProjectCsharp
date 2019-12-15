using System;

namespace SemestreProject.Snake
{
    public abstract class GameObject : BaseObject
    {
        protected Cell cell;

        public void SetCell(Cell value)
        {
            cell = value;
        }
        public Cell GetCell()
        {
            return cell;
        }

        public abstract char GetSymbol();
    }
}