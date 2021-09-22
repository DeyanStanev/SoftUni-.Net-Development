using System;

namespace Re_Volt
{
    class Program
    {
        static void Main(string[] args)
        {
            int sizes = int.Parse(Console.ReadLine());
            int turns = int.Parse(Console.ReadLine());

            char[,] field = new char[sizes, sizes];
            int myRow = 0;
            int myCol = 0;
            bool isWon = false;

            for (int row = 0; row < sizes; row++)
            {
                string input = Console.ReadLine();

                for (int col = 0; col < sizes; col++)
                {
                    field[row, col] = input[col];
                    if (field[row, col] == 'f')
                    {
                        myRow = row;
                        myCol = col;
                    }
                }
            }

            if (turns == 0)
            {
                Console.WriteLine("Player lost!");


                for (int row = 0; row < sizes; row++)
                {
                    for (int col = 0; col < sizes; col++)
                    {
                        Console.Write(field[row, col]);
                    }
                    Console.WriteLine();
                }
                return;
            }

            for (int i = 0; i < turns; i++)
            {

                string comand = Console.ReadLine();

                switch (comand)
                {
                    case "right":

                        field[myRow, myCol] = '-';

                        if (myCol + 1 == sizes)
                        {
                            myCol = 0;
                        }
                        else
                        {
                            myCol++;

                            if (field[myRow, myCol] == 'F')
                            {
                                field[myRow, myCol] = 'f';
                                Console.WriteLine("Player won!");

                                for (int row = 0; row < sizes; row++)
                                {
                                    for (int col = 0; col < sizes; col++)
                                    {
                                        Console.Write(field[row, col]);
                                    }
                                    Console.WriteLine();
                                }

                                isWon = true;
                                return;
                            }
                            else if (field[myRow, myCol] == 'B')
                            {

                                if (myCol + 1 == sizes)
                                {
                                    myCol = 0;
                                }
                                else
                                {
                                    myCol++;
                                }

                            }
                            else if (field[myRow, myCol] == 'T')
                            {
                                myCol--;
                            }
                        }

                        field[myRow, myCol] = 'f';
                        break;


                    case "left":

                        field[myRow, myCol] = '-';

                        if (myCol == 0)
                        {
                            myCol = sizes - 1;
                        }
                        else
                        {
                            myCol--;

                            if (field[myRow, myCol] == 'F')
                            {
                                field[myRow, myCol] = 'f';

                                Console.WriteLine("Player won!");

                                for (int row = 0; row < sizes; row++)
                                {
                                    for (int col = 0; col < sizes; col++)
                                    {
                                        Console.Write(field[row, col]);
                                    }
                                    Console.WriteLine();
                                }

                                isWon = true;
                                return;
                            }

                            else if (field[myRow, myCol] == 'B')
                            {

                                if (myCol - 1 < 0)
                                {
                                    myCol = sizes - 1;
                                }
                                else
                                {
                                    myCol--;
                                }

                            }
                            else if (field[myRow, myCol] == 'T')
                            {
                                myCol++;
                            }
                        }
                        field[myRow, myCol] = 'f';
                        break;

                    case "up":

                        field[myRow, myCol] = '-';

                        if (myRow == 0)
                        {
                            myRow = sizes - 1;
                        }
                        else
                        {
                            myRow--;

                            if (field[myRow, myCol] == 'F')
                            {
                                field[myRow, myCol] = 'f';

                                Console.WriteLine("Player won!");

                                for (int row = 0; row < sizes; row++)
                                {
                                    for (int col = 0; col < sizes; col++)
                                    {
                                        Console.Write(field[row, col]);
                                    }
                                    Console.WriteLine();
                                }

                                isWon = true;
                                Environment.Exit(0);
                            }

                            else if (field[myRow, myCol] == 'B')
                            {

                                if (myRow - 1 < 0)
                                {
                                    myRow = sizes - 1;
                                }
                                else
                                {
                                    myRow--;
                                }

                            }
                            else if (field[myRow, myCol] == 'T')
                            {
                                myRow++;
                            }
                        }
                        field[myRow, myCol] = 'f';
                        break;

                    case "down":

                        field[myRow, myCol] = '-';

                        if (myRow + 1 == sizes)
                        {
                            myRow = 0;
                        }
                        else
                        {
                            myRow++;

                            if (field[myRow, myCol] == 'F')
                            {
                                field[myRow, myCol] = 'f';

                                Console.WriteLine("Player won!");

                                for (int row = 0; row < sizes; row++)
                                {
                                    for (int col = 0; col < sizes; col++)
                                    {
                                        Console.Write(field[row, col]);
                                    }
                                    Console.WriteLine();
                                }

                                isWon = true;
                                return;
                            }
                            else if (field[myRow, myCol] == 'B')
                            {

                                if (myRow + 1 == sizes)
                                {
                                    myRow = 0;
                                }
                                else
                                {
                                    myRow++;
                                }

                            }
                            else if (field[myRow, myCol] == 'T')
                            {
                                myRow--;
                            }
                        }

                        field[myRow, myCol] = 'f';
                        break;

                }

            }

            Console.WriteLine("Player lost!");


            for (int row = 0; row < sizes; row++)
            {
                for (int col = 0; col < sizes; col++)
                {
                    Console.Write(field[row, col]);
                }
                Console.WriteLine();
            }

        }
    }
}
