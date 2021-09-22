using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Guild
{
    public class Guild
    {
        private List<Player> roster;
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Count => roster.Count;

        public Guild(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
            roster = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            if (!roster.Contains(player) && Capacity > roster.Count)
            {
                roster.Add(player);
            }
        }
        public bool RemovePlayer(string name)
        {
            Player current = roster.Where(x => x.Name == name).FirstOrDefault();
            if (current != null)
            {
                roster.Remove(current);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void PromotePlayer(string name)
        {
            Player current = roster.Where(x => x.Name == name).FirstOrDefault();

            if (current!= null)
            {
                if (roster.Contains(current) && current.Rank == "Trial")
                {
                    current.Rank = "Member";
                }
               
            }
           

        }
        public void DemotePlayer(string name)
        {
            Player current = roster.Where(x => x.Name == name).FirstOrDefault();

            if (current != null)
            {
                if (roster.Contains(current) && current.Rank == "Member")
                {
                    current.Rank = "Trial";
                }
            }
           
            
        }
        public Player[] KickPlayersByClass(string clas)
        {
            Player[] kickedPlayers = new Player[]{ };

            if (roster.Count >0)
            {
                kickedPlayers = roster.Where(x => x.Class == clas).ToArray();
               
                roster = roster.Where(x => x.Class != clas).ToList();
            }

            return kickedPlayers;


        }
        public string Report()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Players in the guild: {Name}");
            foreach (Player player in roster)
            {
                sb.AppendLine(player.ToString());
            }
            return sb.ToString().TrimEnd();

        }



    }
}
