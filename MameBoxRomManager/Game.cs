using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MameBoxRomManager
{
    public class Game
    {
        string zipFile;
        string gameName;
        bool inMameBox;

        public Game(string zipFile, string gameName, bool inMameBox)
        {
            this.zipFile = zipFile;
            this.gameName = gameName;
            this.inMameBox = inMameBox;
        }

        public string ZipFile { get => zipFile; set => zipFile = value; }
        public string GameName { get => gameName; set => gameName = value; }
        public bool InMameBox { get => inMameBox; set => inMameBox = value; }

    }
}
