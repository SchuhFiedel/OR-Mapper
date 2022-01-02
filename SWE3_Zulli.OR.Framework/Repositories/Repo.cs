using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework.Repositories
{
    class Repo
    {
        public IDbConnection Connection { get; set; }
        public bool CreateDatabase(string txtDatabase)
        {
            String CreateDatabase;
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            bool IsExits = CheckDatabaseExists(txtDatabase); //Check database exists in sql server.
            if (!IsExits)
            {
                CreateDatabase = "CREATE DATABASE " + txtDatabase + " ; ";
                IDbCommand command = Connection.CreateCommand();
                command.CommandText = (CreateDatabase);
                try
                {
                    Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Please Check Server and Database name. Server and Database name are incorrect :: ", ex.Message);
                    return false;
                }
                finally
                {
                    if (Connection.State == ConnectionState.Open)
                    {
                        Connection.Close();
                    }
                }
                return true;
            }
            return true;
        }

        public bool CheckDatabaseExists(string databaseName)
        {
            string sqlCreateDBQuery;
            bool result = false;

            try
            {
                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);
                using (IDbCommand sqlCmd = Connection.CreateCommand())
                {
                    sqlCmd.CommandText = sqlCreateDBQuery;
                    Connection.Open();
                    object resultObj = sqlCmd.ExecuteScalar();
                    int databaseID = 0;
                    if (resultObj != null)
                    {
                        int.TryParse(resultObj.ToString(), out databaseID);
                    }
                    Connection.Close();
                    result = (databaseID > 0);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
