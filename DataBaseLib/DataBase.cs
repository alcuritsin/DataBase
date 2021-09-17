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

        #endregion

        #region Constructor

        public DataBase()
        {
            _db = new MySqlConnection();
            _command = new MySqlCommand();
        }

        #endregion

        #region Init

        private bool DeserializeJson(string path)
        {
            try
            {
                using var file = new FileStream(path, FileMode.Open);
                _connection = DeserializeAsync<DataBaseConnection>(file).Result;
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Init(string path)
        {
            var res = DeserializeJson(path);

            if (!res) return false;
            _db.ConnectionString = _connection.ToString();
            return true;
        }
        
        public bool Init()
        {
            var res = DeserializeJson("db_connection.json");

            if (!res) return false;
            _db.ConnectionString = _connection.ToString();
            return true;
        }

        #endregion
        
    }
}