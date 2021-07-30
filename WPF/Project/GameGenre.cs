using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Project
{
    class GameGenre
    {
        private static GameGenre _instance;

        public static GameGenre Instance { get { return _instance; } }

        public HashSet<string> Genres = new HashSet<string>();

        public GameGenre()
        {
            _instance = this;
        }
    }
}
