using System;
using System.Collections.Generic;
using System.Linq;

namespace Advanced_Exam___22_Feb_2020
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<int> firstBox = new Queue<int>(Console.ReadLine().Split().Select(int.Parse));
            Stack<int> SecondtBox = new Stack<int>(Console.ReadLine().Split().Select(int.Parse));
            int sum = 0;
            while (firstBox.Count> 0 && SecondtBox.Count>0)
            {
                if ((firstBox.Peek()+SecondtBox.Peek()) % 2 ==0)
                {
                    sum += firstBox.Dequeue() + SecondtBox.Pop();
                }
                else
                {
                    firstBox.Enqueue(SecondtBox.Pop());
                }
            }
            if (firstBox.Count == 0)
            {
                Console.WriteLine("First lootbox is empty");
            }
            else
            {
                Console.WriteLine("Second lootbox is empty");
            }
            if (sum >= 100)
            {
                Console.WriteLine($"Your loot was epic! Value: {sum}");
            }
            else
            {
                Console.WriteLine($"Your loot was poor... Value: {sum}");
            }
        }
    }
}
