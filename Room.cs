using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board
{
    internal class Room
    {
        public int difficulty { get; set; }
        public string size { get; set; }
        public Room()
        {
            this.difficulty = 0;
        }

        public Room(int difficulty)
        {
            this.difficulty = difficulty;
        }
    }
}
