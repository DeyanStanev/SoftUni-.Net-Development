using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Regexer
{
    class Program
    {
        public class Daemons
        {
            public string name { get; set; }
            public int health { get; set; }
            public double damage { get; set; }
        };
        static void Main(string[] args)
        {

            string[] names = Console.ReadLine().Split(", ").Select(x => x.Trim()).ToArray();

            List<Daemons> newList = new List<Daemons>();
            foreach (var item in names)
            {

                MatchCollection health = new Regex(@"[^\d+\-.*\/]").Matches(item);

                int demonHealth = 0;
                for (int i = 0; i < health.Count; i++)
                {
                    demonHealth += char.Parse(health[i].Value);
                }

                MatchCollection matches = new Regex(@"[+\-]{0,1}\d+\.?\d*").Matches(item);

                double damage = 0;

                for (int i = 0; i < matches.Count; i++)
                {
                    damage += double.Parse(matches[i].Value);
                }

                MatchCollection symbols = new Regex(@"[*\/]").Matches(item);

                if (symbols.Count > 0)
                {
                    for (int i = 0; i < symbols.Count; i++)
                    {
                        if (char.Parse(symbols[i].Value) == '*')
                        {
                            damage *= 2;

                        }
                        else
                        {
                            damage /= 2;
                        }
                    }

                }
                Daemons daemons = new Daemons();
                daemons.name = item;
                daemons.health = demonHealth;
                daemons.damage = damage;
                newList.Add(daemons);

            }

            List<Daemons> sortedList = newList.OrderBy(x => x.name).ToList();
            foreach (Daemons daemons in sortedList)
            {
                Console.WriteLine($"{daemons.name} - {daemons.health} health, {daemons.damage:f2} damage");
            }

        }

    }
}

