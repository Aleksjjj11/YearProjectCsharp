using System;

namespace SemestreProject.Snake
{
    public class GameObject : BaseObject
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
    }
}