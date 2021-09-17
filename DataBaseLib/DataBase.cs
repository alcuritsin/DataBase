using System;
using System.IO;
using MySql.Data.MySqlClient;
using static System.Text.Json.JsonSerializer;

namespace DataBaseLib
{
    public class DataBase
    {
        #region Values

        private MySqlConnection _db;
        private MySqlCommand _command;
        private DataBaseConnection _connection;

        public event Action<string> Info;
        public event Action<string> Error;
        public event Action<string> Success; 

        #endregion

        #region Constructor

        public DataBase()
        {
            _db = new MySqlConnection();
            _command = new MySqlCommand();
        }

        public DataBase(Action<string> info, Action<string> error, Action<string> success)
        {
            _db = new MySqlConnection();
            _command = new MySqlCommand();

            Info += info;
            Error += error;
            Success += success;
        }

        #endregion

        #region Init

        private bool DeserializeJson(string path)
        {
            Info?.Invoke("Получение данных из JSON для подключения к БД");
            try
            {
                using var file = new FileStream(path, FileMode.Open);
                _connection = DeserializeAsync<DataBaseConnection>(file).Result;
            }
            catch (Exception)
            {
                Error?.Invoke("Ошибка получения данных из JSON для подключения к БД");
                return false;
            }

            Success?.Invoke("Успешное получение данных из JSON для подключения к БД");
            return true;
        }

        public bool Init(string path)
        {
            Info?.Invoke("Инициализация");
            var res = DeserializeJson(path);

            if (!res)
            {
                Error?.Invoke("Ошибка инициализации");
                return false;
            }
            _db.ConnectionString = _connection.ToString();
            _command.Connection = _db;
            
            Success?.Invoke("Успех инициализации");
            return true;
        }
        
        public bool Init()
        {
            Info?.Invoke("Инициализация");
            var res = DeserializeJson("db_connection.json");

            if (!res)
            {
                Error?.Invoke("Ошибка инициализации");
                return false;
            }
            _db.ConnectionString = _connection.ToString();
            _command.Connection = _db;
            
            Error?.Invoke("Ошибка инициализации");
            return true;
        }

        #endregion

        #region Request

        private bool CheckSql(string sql)
        {
            return !string.IsNullOrEmpty(sql) && !string.IsNullOrWhiteSpace(sql);
        }

        private bool CheckConnectToDb()
        {
            try
            {
                _db.Open();
                Success?.Invoke("Успешное открытие БД");
                return true;
            }
            catch (Exception)
            {
                Error?.Invoke("Ошибка открытия БД");
                return false;
            }
        }
        
        public bool ExecuteSelect(in string sql, out MySqlDataReader outputData)
        {
            Info?.Invoke("Выполнение запроса SELECT");
            
            outputData = null;

            if (!CheckSql(sql))
            {
                Error?.Invoke("Передан пустой запрос SQL");
                return false;
            }

            if (!CheckConnectToDb())
            {
                Error?.Invoke("Ошибка подключения к БД");
                return false;
            }
            
            _command.CommandText = sql;
            outputData = _command.ExecuteReader();
            Info?.Invoke("Выполнение запроса SQL на стороне БД");

            _db.Close();
            Info?.Invoke("Закрытие БД");
            
            return outputData.HasRows;
        }

        public bool ExecuteNotSelect(in string sql, out int countRows)
        {
            Info?.Invoke("Выполнение запроса отличного от SELECT");
            
            countRows = 0;
            
            if (!CheckSql(sql))
            {
                Error?.Invoke("Передан пустой запрос SQL");
                return false;
            }

            if (!CheckConnectToDb())
            {
                Error?.Invoke("Ошибка подключения к БД");
                return false;
            }
            
            _command.CommandText = sql;
            countRows = _command.ExecuteNonQuery();
            Info?.Invoke("Выполнение запроса SQL на стороне БД");
            
            _db.Close();
            Info?.Invoke("Закрытие БД");

            return countRows > 0;
        }

        #endregion
    }
}