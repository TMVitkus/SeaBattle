using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using static SeaBattle.MainWindow;
using static SeaBattle.MainWindow.Ships;

namespace SeaBattle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ClickOnCell += CheckingAndDisplayingComments;

            // Добавляем наши корабли в класс Field
            for (int i = 1; i <= 200; i++)
            {
                cells.Add(new Field((Button)this.FindName($"Cell{i}")));
            }

            // Отправляем список
            ships = new Ships(beginShips);

            SeaBattleDB.CheckingTheDatabase();
        }
        
        // Делегат класса 
        public EventHandler<InitialPositionShip> ClickOnCell;
        // Пременная класса 
        public static WorkingWithCells cells = new WorkingWithCells();
        // По умолчанию делаем расположение корабля горизонтальным
        public Orientation orientation = Orientation.Gorizontal;
        // Перемення для определения попаданий пользователя
        int userHit = 20;
        // По умолчанию корабль однопалубный
        public TypeShip typeShip { get; set; } = TypeShip.oneDeck;
        // Список кораблей в котром указан их размер и количество
        List<Ship> beginShips = new List<Ship>
        {
            new Ship { Type = TypeShip.oneDeck, CountShip = 4 },
            new Ship { Type = TypeShip.twoDeck, CountShip=3 },
            new Ship { Type = TypeShip.threeDeck, CountShip=2 },
            new Ship { Type = TypeShip.fourDeck, CountShip=1 }
        };
        // Переменная класса
        public Ships ships;
        // Переменная класса
        private Computer enemy;
        DateTime dateTime;
        DispatcherTimer timer = new DispatcherTimer();
        public int numberOfShots = 0;
        public int numberOfHits = 0;
        public string timerRecording = "";

        /// <summary>
        /// Расположение корабля горизонтальное
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HorizontalShip(object sender, RoutedEventArgs e)
        {
            orientation = Orientation.Gorizontal;
        }
        /// <summary>
        /// Расположение корабля вертикальное
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerticalShip(object sender, RoutedEventArgs e)
        {
            orientation = Orientation.Vertical;
        }

        /// <summary>
        /// Выбор судна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShipSelection(object sender, RoutedEventArgs e)
        {
            // Конвертируем данные о палубе корабля и прасваиваем
            typeShip = (TypeShip)Convert.ToInt32((sender as RadioButton).Content);
        }

        /// <summary>
        /// Метод, который определяет куда ткнул пользователь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArrangementOfShips(object sender, RoutedEventArgs e)
        {
            // С помощью метода Invoke ищем и присваиваем "координаты" переменной класса 
            ClickOnCell?.Invoke(this, new InitialPositionShip { StartCell = cells.GetCellByButton(sender as Button) });
        }

        /// <summary>
        /// Метод, выводящий статистику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsOfAllGames(object sender, RoutedEventArgs e)
        {
            Statistic statistic = new Statistic();
            statistic.ShowDialog();
        }

        /// <summary>
        /// Начинаем игру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            // Очищаем строку выведения комментариев
            labelCommentsOnThePlacement.Content = "";
            // Включаем таймер
            GameTimer();
            // Делаем кнопки не работающими
            buttonPlay.IsEnabled = false;
            buttonAutoPlacement.IsEnabled = false;
            buttonClear.IsEnabled = false;
            // Растягиваем label по полю чтобы пользователь не мог нажимать на своё поле во время битвы
            labelDeactivationTheUser.Height = 300;
            labelDeactivationTheUser.Width = 300;
            // Уменьшаем label чтобы пользователь мог нажимать на поле компьютера во время битвы
            labelDeactivationTheComputer.Height = 0;
            labelDeactivationTheComputer.Width = 0;
            ClickOnCell += MovesAndVerificationInBattle;
            // Расставляем корабли на вражеском поле
            AutomaticPlacementOfShips(new Ships(beginShips), 2);
            enemy = new Computer();
            // Добавляем в базу данных новую игру
            SeaBattleDB.CountingForStatistics("NumberOfGames");
        }

        /// <summary>
        /// Создаём новую игру
        /// </summary>
        public void NewGame()
        {
            labelTimer.Content = "00:00";
            userHit = 20;
            Computer.computerHit = 20;
            numberOfShots = 0;
            numberOfHits = 0;
            timerRecording = "";
            // Очищаем строку выведения комментариев
            labelCommentsOnThePlacement.Content = "";
            labelCommentsOfTheFight.Content = "";
            // Очищаем поле
            ClearField();
            // Меняем работу кнопок
            buttonPlay.IsEnabled = false;
            buttonAutoPlacement.IsEnabled = true;
            buttonClear.IsEnabled = true;
            // Уменьшаем label по полю чтобы пользователь мог расставить корабли
            labelDeactivationTheUser.Height = 0;
            labelDeactivationTheUser.Width = 0;
            // Растягиваем label чтобы пользователь не мог нажимать на поле компьютера
            labelDeactivationTheComputer.Height = 300;
            labelDeactivationTheComputer.Width = 300;

            ClickOnCell += CheckingAndDisplayingComments;
            ClickOnCell -= MovesAndVerificationInBattle;
        }

        /// <summary>
        /// Метод проверки и вывод комментариев
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckingAndDisplayingComments(object sender, InitialPositionShip e)
        {
            // Очищаем строку выведения комментариев
            labelCommentsOnThePlacement.Content = "";
            // Если количество не равно нулю, проводим проверку и ставим корабль
            if (ships[typeShip].CountShip != 0)
            {
                try
                {
                    // Вызов метода
                    SupplyShip(e.StartCell, typeShip, orientation);
                }
                catch (IndexOutOfRangeException ex)
                {
                    // Если индекс вне диапазона выводим об этом сообщение
                    labelCommentsOnThePlacement.Content = "Выход за границы поля!";
                }
                catch (Exception ex)
                {
                    // При другой ошибке тоже выводим комментарий
                    labelCommentsOnThePlacement.Content = ex.Message;
                }
                // Отнимаем количество
                ships[typeShip].CountShip--;
            }
            else
            {
                // Вывод комментария
                labelCommentsOnThePlacement.Content = "Такие корабли закончились!";
            }
            // Если кораблей не осталось, делаем кнопку рабочей
            if (ships.GetCountShips() == 0)
            {
                buttonPlay.IsEnabled = true;
            }

        }

        public void SupplyShip(Field startPosition, TypeShip ship, Orientation orientation)
        {
            // Очищаем строку выведения комментариев
            labelCommentsOnThePlacement.Content = "";
            string name;
            do
            {
                // Инициализируем новый экземпляр структуры и конвертируем в строку
                name = Guid.NewGuid().ToString();
            } while (!CheckAvailabilityName(name));

            // Присваиваем позицию корабля
            int position = cells.GetIndexCell(startPosition.PushButton);
            // Если корабль расположен вертикально
            if (orientation == Orientation.Vertical)
            {
                for (int i = 0, j = 0; i < (int)ship; i++, j += 10)
                {
                    if (cells.GetIndexCell(startPosition.PushButton) < 100)
                    {
                        if ((position + j) > 99)
                        {
                            throw new IndexOutOfRangeException();
                        }                             
                    }
                    else
                    {
                        if ((position + j) > 199)
                        {
                            throw new IndexOutOfRangeException();
                        }                   
                    }

                }
                if (!CheckingTheVertical(position, ship))
                {
                    throw new Exception("Рядом корабль");
                    labelCommentsOnThePlacement.Content = "Рядом корабль";
                }


                for (int i = 0, j = 0; i < (int)ship; i++, j += 10)
                {
                    // Присваиваем имя 
                    cells[cells.GetIndexCell(startPosition.PushButton) + j].NameShip = name;
                    if (cells.GetIndexCell(startPosition.PushButton) < 100)
                    {
                        // Если корабль находится на поле пользователя окрашиваем 
                        cells[cells.GetIndexCell(startPosition.PushButton) + j].Color = Color.FromArgb(240, 70, 130, 180);
                    }
                    // Определяем место в клетке занятым
                    cells[cells.GetIndexCell(startPosition.PushButton) + j].Busy = true;
                }
            }
            else
            {
                for (int i = 1; i < (int)ship; i++)
                {
                    if ((position + i) % 10 == 0)
                        throw new IndexOutOfRangeException();
                }
                if (!CheckingTheGorizontal(position, ship))
                {
                    throw new Exception("Рядом корабль");
                    labelCommentsOnThePlacement.Content = "Рядом корабль";
                }

                for (int i = 0; i < (int)ship; i++)
                {
                    // Присваиваем имя 
                    cells[cells.GetIndexCell(startPosition.PushButton) + i].NameShip = name;
                    if (cells.GetIndexCell(startPosition.PushButton) < 100)
                    {
                        // Если корабль находится на поле пользователя окрашиваем 
                        cells[cells.GetIndexCell(startPosition.PushButton) + i].Color = Color.FromArgb(240, 70, 130, 180);
                    }
                    // Определяем место в клетке занятым
                    cells[cells.GetIndexCell(startPosition.PushButton) + i].Busy = true;
                }
            }
        }

        public bool CheckAvailabilityName(string name)
        {
            for (int i = 0; i < cells.GetCountCell(); i++)
            {
                if (cells[i].NameShip == name)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяем доступность по вертикали
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="ship"></param>
        /// <returns></returns>
        public bool CheckingTheVertical(int startPosition, TypeShip ship)
        {
            // Проверяем верх
            // Если стартовая позиция будет выходить за пределы верхнего поля
            if ((startPosition - 10) >= 0)
            {
                if (startPosition != 0 && (startPosition - 1) % 10 != 9)
                {
                    if (cells[startPosition - 1 - 10].Busy)
                    {
                        return false;
                    }                           
                }
                if (cells[startPosition - 10].Busy)
                {
                    return false;
                }                   
                if ((startPosition + 1) % 10 != 0)
                {
                    if (cells[startPosition - 10 + 1].Busy)
                    {
                        return false;
                    }                     
                }
            }
            // Проверяем низ
            if ((startPosition + (int)ship * 10) <= 199)
            {
                if ((startPosition + (int)ship * 10 - 1) % 10 != 9)
                {
                    if (cells[startPosition - 1 + (int)ship * 10].Busy)
                    {
                        return false;
                    }
                }
                if (cells[startPosition + (int)ship * 10].Busy)
                {
                    return false;
                }
                if ((startPosition + 1 + (int)ship * 10) % 10 != 0)
                {
                    if (cells[startPosition + (int)ship * 10 + 1].Busy)
                    {
                        return false;
                    }
                }
            }
            // Проверяем боковые стороны
            for (int i = 0, j = 0; i < (int)ship; i++, j += 10)
            {
                if (startPosition != 0 && (startPosition - 1) % 10 != 9)
                    if (cells[startPosition - 1 + j].Busy)
                    {
                        return false;
                    }
                if ((startPosition + 1) % 10 != 0)
                {
                    if (cells[startPosition + 1 + j].Busy)
                    {
                        return false;
                    }
                }                  
                if (cells[startPosition + j].Busy)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Проверяем доступность по горизонтали
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="ship"></param>
        /// <returns></returns>
        private bool CheckingTheGorizontal(int startPosition, TypeShip ship)
        {
            // Проверяем лево
            if (startPosition != 0 && (startPosition - 1) % 10 != 9)
            {
                if (startPosition - 10 > 0)
                {
                    if (cells[startPosition - 10 - 1].Busy)
                    {
                        return false;
                    }
                }
                if (startPosition != 0 && cells[startPosition - 1].Busy)
                {
                    return false;
                }
                if (startPosition + 10 < 199)
                {
                    if (cells[startPosition + 10 - 1].Busy)
                    {
                        return false;
                    }
                }
            }
            // Проверяем право
            if ((startPosition + (int)ship) % 10 != 0)
            {
                if (startPosition - 10 > 0)
                {
                    if (cells[startPosition - 10 + (int)ship].Busy)
                    {
                        return false;
                    }
                }
                if (cells[startPosition + (int)ship].Busy)
                {
                    return false;
                }
                if (startPosition + 10 < 199)
                {
                    if (cells[startPosition + 10 + (int)ship].Busy)
                    {
                        return false;
                    }
                }
            }
            // Проверяем стороны
            for (int i = 0; i < (int)ship; i++)
            {
                if (startPosition - 10 >= 0)
                {
                    if (cells[startPosition + i - 10].Busy)
                    {
                        return false;
                    }
                }                  
                if (startPosition + 10 <= 199)
                {
                    if (cells[startPosition + i + 10].Busy)
                    {
                        return false;
                    }
                }   
                if (cells[startPosition + i].Busy)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Включаем таймер
        /// </summary>
        public void GameTimer()
        {
            dateTime = DateTime.Now;
            // Назначаем интервал
            timer.Interval = new TimeSpan(0, 0, 1);
            // Вызываем метод
            timer.Tick += new EventHandler(TickTimer);
            // Включаем таймер
            timer.Start();
        }
        /// <summary>
        /// Подсчёт и вывод таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TickTimer(object sender, EventArgs e)
        {
            long tick = DateTime.Now.Ticks - dateTime.Ticks;
            DateTime stopWatch = new DateTime();
            stopWatch = stopWatch.AddTicks(tick);
            // Выводим таймер
            labelTimer.Content = string.Format("{0:mm:ss}", stopWatch);
            // Присваиваем для базы данных
            timerRecording = labelTimer.Content.ToString();
        }

        /// <summary>
        /// Метод очистки поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearTheField(object sender, RoutedEventArgs e)
        {
            buttonAutoPlacement.IsEnabled = true;
            // Вызов метода
            ClearField();
            // Передаём список
            ships = new Ships(beginShips);
            // Делаем кнопку "Играть" не рабочей
            buttonPlay.IsEnabled = false;
            if (ClickOnCell == null)
            {
                ClickOnCell += CheckingAndDisplayingComments;
            }
        }

        /// <summary>
        /// Метод очищает поле
        /// </summary>
        public void ClearField()
        {
            for (int i = 0; i < cells.GetCountCell(); i++)
            {
                // Убираем цвет
                Color color = Color.FromArgb(0, 211, 211, 211);
                cells[i].Color = color;
                // Клетка становится не занятой
                cells[i].Busy = false;
                // В корабль не стреляли
                cells[i].Shooting = false;
            }
        }

        /// <summary>
        /// Авторасстановка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoPlacement(object sender, RoutedEventArgs e)
        {
            buttonAutoPlacement.IsEnabled = false;
            // Вызов метода
            AutomaticPlacementOfShips(ships, 1);
            // На кнопку "Играть" можно нажать
            buttonPlay.IsEnabled = true;
            ClickOnCell -= CheckingAndDisplayingComments;
        }

        /// <summary>
        /// Авторасстановка кораблей
        /// </summary>
        /// <param name="ships"></param>
        /// <param name="player"></param>
        public void AutomaticPlacementOfShips(Ships ships, int player)
        {
            Random random = new Random();
            // player == 1 - это пользователь
            // player == 2 - это компьютер
            if (player == 1)
            {
                // Очищаем поле
                ClearField();
            }
            int count = 0;
            foreach (var item in ships.GetShips())
            {
                for (int i = 0; i < item.CountShip; i++)
                {
                    count = 0;
                    while (true)
                    {
                        Field cell = new Field();
                        if (player == 1)
                        {
                            // Рандомно подбирается клетка
                            cell = cells[random.Next(0, 100)];
                        }
                        else if (player == 2)
                        {
                            cell = cells[random.Next(100, 200)];
                        }
                        if (cell.Busy)
                        {
                            continue;
                        }
                        // Рандомно подбирается ориентация корабля
                        var orientation = (Orientation)random.Next(0, 2);
                        try
                        {
                            SupplyShip(cell, item.Type, orientation);
                        }
                        catch
                        {
                            count++;
                            if (player == 1)
                            {
                                if (count >= 100)
                                {
                                    // Если корабль не подходит очищаем поле
                                    ClearField();
                                }
                            }
                            else if (player == 2)
                            {
                                if (count >= 200)
                                {
                                    // Если корабль не подходит очищаем поле
                                    ClearField();
                                }
                            }
                            continue;
                        }
                        break;
                    }
                }
            }
        }

        private void MovesAndVerificationInBattle(object sender, InitialPositionShip e)
        {
            // Очищаем строку выведения комментариев
            labelCommentsOfTheFight.Content = "";
            // Проверяем стрелиля ли в данную клетку
            if (e.StartCell.Shooting)
            {
                labelCommentsOfTheFight.Content = "Вы сюда уже стреляли!";
            }
            // Добавляем запись в базу данных
            SeaBattleDB.CountingForStatistics("NumberOfShots");
            // Добавляем выстрел в общий счёт
            numberOfShots++;
            if (e.StartCell.Busy)
            {
                // Меняем интерфейс кнопки
                e.StartCell.Hitting();
                // Отнимаем от общего количества
                userHit--;
                // Добавляем запись в базу данных
                SeaBattleDB.CountingForStatistics("NumberOfHits");
                // Добавляем выстрел в общий счёт
                numberOfHits++;
                // Если корабль полностью полтопили, то выводим комментарий
                if (cells.GetCountLivingShipsByName(e.StartCell.NameShip) == 0)
                {
                    labelCommentsOfTheFight.Content = "Вы потопили корабль!";
                }
            }
            else
            {
                e.StartCell.Miss();
            }

            // Если пользователь потопил все корабли
            if (userHit == 0)
            {
                // Останавливаем таймер
                timer.Stop();
                // Добавляем запись в базу данных
                SeaBattleDB.CountingForStatistics("NumberOfWins");
                if (MessageBox.Show("Вы победили! Хотите увидеть результат игры?", "Поздравляю", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    // Добавляем запись в базу данных
                    SeaBattleDB.AddingResults(numberOfShots, numberOfHits, timerRecording);
                    // Открываем окно
                    GameResult gameResult = new GameResult();
                    gameResult.ShowDialog();
                    if (MessageBox.Show("Хотите начать игру заново?", "._?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        NewGame();
                    }
                    else
                    {
                        // Закрываем приложение
                        this.Close();
                    }
                }
                else
                {
                    if (MessageBox.Show("Хотите начать игру заново?", "._?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        NewGame();
                    }
                    else
                    {
                        // Закрываем приложение
                        this.Close();
                    }                    
                }
            }
            // Удары корабля
            enemy.Shoot();
            // Если компьютер потопил все корабли
            if (Computer.computerHit == 0)
            {
                // Останавливаем таймер
                timer.Stop();
                if (MessageBox.Show("Вы проиграли! Хотите увидеть результат игры?", "Сочувствуем", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    // Добавляем запись в базу данных
                    SeaBattleDB.AddingResults(numberOfShots, numberOfHits, timerRecording);
                    // Открываем окно
                    GameResult gameResult = new GameResult();
                    gameResult.ShowDialog();
                    if (MessageBox.Show("Хотите начать игру заново?", "._?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        NewGame();
                    }
                    else
                    {
                        // Закрываем приложение
                        this.Close();
                    }
                }
                else
                {
                    if (MessageBox.Show("Хотите начать игру заново?", "._?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        NewGame();
                    }
                    else
                    {
                        // Закрываем приложение
                        this.Close();
                    }
                }
            }
        }       
    }
}