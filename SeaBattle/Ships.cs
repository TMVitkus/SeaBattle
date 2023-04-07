using System.Collections.Generic;

namespace SeaBattle
{
    public partial class MainWindow
    {
        public partial class Ships
        {
            // Список класса
            List<Ship> ships = new List<Ship>();
            public Ships(List<Ship> ships)
            {
                for (int i = 0; i < ships.Count; i++)
                {
                    // Добавляем в класс тип и количество кораблей
                    this.ships.Add(new Ship { Type = ships[i].Type, CountShip = ships[i].CountShip });
                }

            }
            /// <summary>
            /// Получаем количестов кораблей
            /// </summary>
            /// <returns></returns>
            public int GetCountShips()
            {
                int count = 0;
                foreach (var item in this.ships)
                {
                    count += item.CountShip;
                }
                return count;
            }
            /// <summary>
            /// Получаем и отправляем корабли
            /// </summary>
            /// <returns></returns>
            public List<Ship> GetShips()
            {
                return ships;
            }
            /// <summary>
            /// Объявлем индексаторы
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Ship this[TypeShip index]
            {
                get
                {
                    foreach (var item in ships)
                    {
                        if (item.Type == index)
                        {
                            return item;
                        }
                    }
                    return null;
                }
                set
                {
                    for (int i = 0; i < ships.Count; i++)
                    {
                        if (ships[i].Type == index)
                        {
                            ships[i] = value;
                        }
                    }
                }
            }
        }
    }
}