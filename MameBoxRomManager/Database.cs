using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace MameBoxRomManager
{
    class Database
    {
        //Database file location
        private string dbFilename = "mbrm.sqlite";

        //SQLIteConnection
        SQLiteConnection dbConn;

        public Database()
        {
            //Set if the database need to be initialized
            bool doInit = false;
            //Looking for a database file;
            if (!File.Exists(this.dbFilename))
            {
                SQLiteConnection.CreateFile(this.dbFilename);
                doInit = true;
            }
            //Initializing database connection
            this.dbConn = new SQLiteConnection("Data Source=" + this.dbFilename + ";Version=3;");
            if (doInit)
            {
                this.initDatabase();
            }
        }

        private void initDatabase()
        {
            this.executeQuery("CREATE TABLE `games` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `zipName` TEXT, `gameName` TEXT, `inMamebox` INTEGER);");
            this.executeQuery("CREATE TABLE `settings` (`key` TEXT, `value` TEXT);");
            this.executeQuery("INSERT INTO settings(key, value) VALUES('dbversion', '1')");
            this.executeQuery("INSERT INTO settings(key, value) VALUES('fullsetDir', '')");
            this.executeQuery("INSERT INTO settings(key, value) VALUES('mameboxDir', '')");
            this.executeQuery("INSERT INTO settings(key, value) VALUES('xmlFileDir', '')");
        }

        public bool executeQuery(string sql)
        {
            int returnVal;
            this.dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, this.dbConn);
            returnVal = cmd.ExecuteNonQuery();
            this.dbConn.Close();
            if(returnVal > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getSetting(string key)
        {
            string returnValue = "";
            this.dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT value FROM settings WHERE key = '" + key + "'",this.dbConn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                returnValue = rdr["value"].ToString();
            }
            this.dbConn.Close();
            return returnValue;
        }

        public bool setSetting(string key, string value)
        {
            return this.executeQuery("UPDATE settings SET value = '"+value+"' WHERE key = '"+key+"'");
        }

        public void addGame(string zipName, string gameName)
        {
            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO games(zipName, gameName,inMamebox) VALUES(@zipName, @gameName, 0)", this.dbConn);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("zipName", zipName));
            cmd.Parameters.Add(new SQLiteParameter("gameName", gameName));
            this.dbConn.Open();
            cmd.ExecuteNonQuery();
            this.dbConn.Close();

        }

        public void updateGameEntry(string zipFile, int isPresent)
        {
            this.executeQuery("UPDATE games SET inMamebox = 1 WHERE zipName = '"+zipFile+"'");
        }


    }
}
