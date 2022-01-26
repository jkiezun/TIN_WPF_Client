using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Models
{
    public class Lobby
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayersCount { get; set; }
        public int hasPassword { get; set; }
    }
}
