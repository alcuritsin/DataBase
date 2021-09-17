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
            _command.Connection = _db;
            return true;
        }
        
        public bool Init()
        {
            var res = DeserializeJson("db_connection.json");

            if (!res) return false;
            _db.ConnectionString = _connection.ToString();
            _command.Connection = _db;
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool ExecuteSelect(in string sql, out MySqlDataReader outputData)
        {
            outputData = null;

            if (!CheckSql(sql))
            {
                return false;
            }

            if (!CheckConnectToDb())
            {
                return false;
            }
            
            _command.CommandText = sql;
            outputData = _command.ExecuteReader();

            _db.Close();

            return outputData.HasRows;
        }

        public bool ExecuteNotSelect(in string sql, out int countRows)
        {
            countRows = 0;
            
            if (!CheckSql(sql))
            {
                return false;
            }

            if (!CheckConnectToDb())
            {
                return false;
            }
            
            _command.CommandText = sql;
            countRows = _command.ExecuteNonQuery();

            return countRows > 0;
        }

        #endregion
    }
}