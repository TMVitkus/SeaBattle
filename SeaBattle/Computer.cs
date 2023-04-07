using System;

namespace SeaBattle
{
    public partial class MainWindow
    {
        class ShotsFiredAtShips
        {
            public bool Hit { get; set; } = false;
            public int StartPositionShip { get; set; }
            public bool left { get; set; } = true;
            public bool right { get; set; } = true;
            public bool down { get; set; } = true;
            public bool top { get; set; } = true;
            public int CountShot { get; set; }
            public void Clear()
            {
                Hit = false;
                left = true;
                down = true;
                right = true;
                top = true;
                CountShot = 0;
            }

        }
        public class Computer
        {
            // Переменная подсчитывает сколько раз попал компьютер 
            public static int computerHit = 20;
            public Computer()
            {
            }           
            private ShotsFiredAtShips lastShot = new ShotsFiredAtShips();
            public enum Side
            {
                left,
                right,
                top,
                down,
                none
            }
            // Удары
            private bool Hit(int index)
            {
                if (cells[index].Busy)
                {
                    computerHit--;
                    cells[index].Hitting();                  
                    if (cells.GetCountLivingShipsByName(cells[index].NameShip) == 0)
                    {
                        lastShot.Clear();
                    }
                    else
                    {
                        lastShot.Hit = true;
                        lastShot.CountShot++;
                    }
                    return true;
                }
                else
                {
                    cells[index].Miss();
                    lastShot.CountShot = 1;
                    return false;
                }
            }
            public bool CheckingTheLocation(int index, Side side)
            {
                if (index < 0)
                {
                    return false;
                }
                if (cells[index].Shooting)
                {
                    return false;
                }
                // Проверяем верх
                if (index - 10 >= 0)
                {
                    // Верхний левый угол
                    if (index % 10 != 0 && index != 0)
                    {
                        if (cells[index - 10 - 1].Busy && cells[index - 10 - 1].Shooting)
                        {
                            return false;
                        }
                    }
                    // Верхний
                    if (cells[index - 10].Busy && cells[index - 10].Shooting && side != Side.down)
                    {
                        return false;
                    }
                    // Верхний правый
                    if (index % 10 != 9 && index != 99)
                    {
                        if (cells[index - 10 + 1].Busy && cells[index - 10 + 1].Shooting)
                        {
                            return false;
                        }
                    }
                }
                // Проверяем низ
                if (index + 10 <= 99)
                {
                    if (index % 10 != 0 && index != 0)
                    {
                        if (cells[index + 10 - 1].Busy && cells[index + 10 - 1].Shooting)
                        {
                            return false;
                        }
                    }
                    // Низ
                    if (cells[index + 10].Busy && cells[index + 10].Shooting && side != Side.top)
                    {
                        return false;
                    }
                    if (index % 10 != 9 && index != 199)
                    {
                        if (cells[index + 10 + 1].Busy && cells[index + 10 + 1].Shooting)
                        {
                            return false;
                        }
                    }
                }
                if (index % 10 != 0 && side != Side.right)
                {
                    if (cells[index - 1].Busy && cells[index - 1].Shooting)
                    {
                        return false;
                    }
                }
                if (index % 10 != 9 && side != Side.left)
                {
                    if (cells[index + 1].Busy && cells[index + 1].Shooting)
                    {
                        return false;
                    }
                }
                return true;
            }
            /// <summary>
            /// Метод выбирает куда стрельнуть
            /// </summary>
            public void Shoot()
            {
                Random random = new Random();
                if (!lastShot.Hit)
                {
                    while (true)
                    {
                        // Выбираем рандомную клетку
                        int index = random.Next(0, 100);
                        if (cells[index].Shooting)
                        {
                            continue;
                        }
                        else
                        {
                            if (cells[index].Busy)
                            {
                                // Отнимаем от общего числа
                                computerHit--;
                                cells[index].Hitting();
                                if (cells.GetCountLivingShipsByName(cells[index].NameShip) == 0)
                                {
                                    lastShot.Clear();
                                }
                                else
                                {
                                    // В кнопку стреляли
                                    lastShot.Hit = true;
                                    // Добавляем в количестов
                                    lastShot.CountShot++;
                                    // Присваиваем индекс
                                    lastShot.StartPositionShip = index;
                                }
                            }
                            else
                            {
                                cells[index].Miss();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    // Проверка лева
                    if (lastShot.left)
                    {
                        if (CheckingTheLocation(lastShot.StartPositionShip - lastShot.CountShot, Side.left))
                        {
                            if ((lastShot.StartPositionShip - lastShot.CountShot) % 10 == 9 || lastShot.StartPositionShip - lastShot.CountShot < 0)
                            {
                                lastShot.left = false;
                            }
                            else
                            {
                                if (Hit(lastShot.StartPositionShip - lastShot.CountShot))
                                {

                                    if (cells.GetCountLivingShipsByName(cells[lastShot.StartPositionShip].NameShip) != 0)
                                    {
                                        lastShot.top = false;
                                        lastShot.down = false;
                                    }
                                }
                                else
                                {
                                    lastShot.left = false;
                                }
                            }
                        }
                        lastShot.left = false;
                    }
                    // Проверяем вверх
                    if (lastShot.top)
                    {
                        if (CheckingTheLocation(lastShot.StartPositionShip - lastShot.CountShot * 10, Side.top))
                        {
                            if (lastShot.StartPositionShip - lastShot.CountShot * 10 % 10 == 0 || lastShot.StartPositionShip - lastShot.CountShot * 10 < 0)
                            {
                                lastShot.top = false;
                            }
                            else
                            {

                                if (Hit(lastShot.StartPositionShip - lastShot.CountShot * 10))
                                {

                                    if (cells.GetCountLivingShipsByName(cells[lastShot.StartPositionShip].NameShip) != 0)
                                    {
                                        lastShot.left = false;
                                        lastShot.right = false;
                                    }
                                }
                                else
                                {
                                    lastShot.top = false;
                                }
                            }
                        }
                        lastShot.top = false;
                    }
                    // Проверям право
                    if (lastShot.right)
                    {
                        if (CheckingTheLocation(lastShot.StartPositionShip + lastShot.CountShot, Side.right))
                        {
                            if ((lastShot.StartPositionShip + lastShot.CountShot) % 10 == 9 || lastShot.StartPositionShip + lastShot.CountShot > 199)
                            {
                                lastShot.right = false;
                            }
                            else
                            {
                                if (Hit(lastShot.StartPositionShip + lastShot.CountShot))
                                {
                                    if (cells.GetCountLivingShipsByName(cells[lastShot.StartPositionShip].NameShip) != 0)
                                    {
                                        lastShot.top = false;
                                        lastShot.down = false;
                                    }
                                }
                                else
                                {
                                    lastShot.right = false;
                                }
                            }
                        }
                        lastShot.right = false;
                    }
                    // Провнряем низ
                    if (lastShot.down)
                    {
                        if (CheckingTheLocation(lastShot.StartPositionShip + lastShot.CountShot * 10, Side.down))
                        {
                            if (lastShot.StartPositionShip + lastShot.CountShot * 10 % 10 == 9 || lastShot.StartPositionShip + lastShot.CountShot * 10 > 199)
                            {
                                lastShot.down = false;
                            }
                            else
                            {

                                if (Hit(lastShot.StartPositionShip + lastShot.CountShot * 10))
                                {

                                    if (cells.GetCountLivingShipsByName(cells[lastShot.StartPositionShip].NameShip) != 0)
                                    {
                                        lastShot.left = false;
                                        lastShot.right = false;
                                    }
                                }
                                else
                                {
                                    lastShot.down = false;
                                }
                            }
                        }
                        lastShot.down = false;
                    }
                    lastShot.Clear();
                }
            }
        }
    }
}