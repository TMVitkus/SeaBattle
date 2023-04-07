using System.Data.SqlClient;

namespace SeaBattle
{
    class SeaBattleDB
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SeaBattleDB; Integrated Security=True;";
        static SqlConnection connection = new SqlConnection(connectionString);
        /// <summary>
        /// Проверяем наличие базы данных
        /// </summary>
        public static void CheckingTheDatabase()
        {
            try
            {
                CreatingADatabase();
                connection.Close();
                try
                {
                    CreatingTables();
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            catch
            {
                try
                {
                    CreatingTables();
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Создание базы данных
        /// </summary>
        public static void CreatingADatabase()
        {
            string connectionStringForDB = @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=master; Integrated Security=True;";
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionStringForDB);
            connection.Open();
            SqlCommand command = new SqlCommand();
            // Выполняем команду на создание базы данных
            command.CommandText = "CREATE DATABASE SeaBattleDB;";
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Создание таблиц
        /// </summary>
        public static void CreatingTables()
        {
            // Создание подключения
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            sqlConnection.Open();
            // Выполняем команду на создание базы данных
            command.CommandText = "CREATE TABLE GameResult" +
                                  "(" +
                                     "Id INT PRIMARY KEY IDENTITY," +
                                     "NumberOfShots INT," +
                                     "NumberOfHits INT," +
                                     "Timer TIME" +
                                  ")" +
                                  "CREATE TABLE Statistic" +
                                  "(" +
                                     "NumberOfWins INT," +
                                     "NumberOfGames INT," +
                                     "NumberOfShots INT," +
                                     "NumberOfHits INT" +
                                  ");" +
                                  "INSERT INTO Statistic VALUES (0, 0, 0, 0);";
            command.Connection = sqlConnection;
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        /// <summary>
        /// Добавлем результат игры
        /// </summary>
        /// <param name="numberOfShots"></param>
        /// <param name="numberOfHits"></param>
        /// <param name="timerRecording"></param>
        public static void AddingResults(int numberOfShots, int numberOfHits, string timerRecording)
        {
            // Создание подключения
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            sqlConnection.Open();
            command.CommandText = $"INSERT INTO GameResult VALUES ({numberOfShots}, {numberOfHits}, '00:{timerRecording}');";
            command.Connection = sqlConnection;
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        /// <summary>
        /// Прибавлем значение в статистику
        /// </summary>
        /// <param name="name"></param>
        public static void CountingForStatistics(string name)
        {
            // Создание подключения
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            sqlConnection.Open();
            command.CommandText = $"UPDATE Statistic SET {name} = {name} + 1";
            command.Connection = sqlConnection;
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        /// <summary>
        /// Вывод статистики
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string StatisticsOutput(string columnName)
        {
            // Создание подключения
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            sqlConnection.Open();
            command.CommandText = $"SELECT {columnName} FROM Statistic";
            command.Connection = sqlConnection;
            string value = command.ExecuteScalar().ToString();
            sqlConnection.Close();
            return value;
        }

        /// <summary>
        /// Вывод результата игры
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GameResultOutput(string columnName)
        {
            // Создание подключения
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            sqlConnection.Open();
            command.CommandText = $"SELECT TOP 1 {columnName} FROM GameResult ORDER BY Id DESC";
            command.Connection = sqlConnection;
            string value = command.ExecuteScalar().ToString();
            sqlConnection.Close();
            return value;
        }
    }
}