using System.Collections.Generic;
using System.Windows.Controls;

namespace SeaBattle
{
    public partial class MainWindow
    {
        public class WorkingWithCells
        {
            // Список с кнопками
            public List<Field> buttons = new List<Field>();

            /// <summary>
            /// Метод, который подсчитывает на какую кнопку нажали
            /// </summary>
            /// <param name="button"></param>
            /// <returns></returns>
            public Field GetCellByButton(Button button)
            {
                return buttons.Find(b => b.PushButton == button);
            }

            /// <summary>
            /// Метод используется для поиска индекса первого вхождения
            /// </summary>
            /// <param name="button"></param>
            /// <returns></returns>
            public int GetIndexCell(Button button)
            {
                return buttons.IndexOf(buttons.Find(p => p.PushButton == button));
            }

            /// <summary>
            /// Метод возвращает количество кнопок
            /// </summary>
            /// <returns></returns>
            public int GetCountCell()
            {
                return buttons.Count;
            }

            /// <summary>
            /// Метод объявляет индексаторы
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Field this[int index]
            {
                get { return buttons[index]; }
                set { buttons[index] = value; }
            }

            /// <summary>
            /// Метод добляющий наши корабли
            /// </summary>
            /// <param name="item"></param>
            public void Add(Field item)
            {
                buttons.Add(item);
            }

            /// <summary>
            /// Получаем количество живых кораблей 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public int GetCountLivingShipsByName(string name)
            {
                int count = 0;
                foreach (var item in buttons)
                {
                    // Если в корабль не стреляли добавлем количество
                    if (item.NameShip == name && !item.Shooting)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}