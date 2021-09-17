namespace DataBaseLib
{
    public class DataBaseConnection
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Uid { get; set; }
        public string Pwd { get; set; }

        public override string ToString()
        {
            //Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
            return $"Server={Server};Database={Database};Uid={Uid};Pwd={Pwd};";
        }
    }
}