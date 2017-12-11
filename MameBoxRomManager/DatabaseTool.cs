using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace MameBoxRomManager
{
    class DatabaseTool : Database
    {
        //Return setting value
        public string getSetting(string key)
        {
            string returnValue = "";
            this.dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT value FROM settings WHERE key = '" + key + "'", this.dbConn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                returnValue = rdr["value"].ToString();
            }
            this.dbConn.Close();
            return returnValue;
        }

        //Setting a setting lol
        public bool setSetting(string key, string value)
        {
            return this.executeQuery("UPDATE settings SET value = '" + value + "' WHERE key = '" + key + "'");
        }

        //Add game in DB
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

        //Update a game status in db
        public void updateGameEntry(string zipFile, int isPresent)
        {
            this.executeQuery("UPDATE games SET inMamebox = '" + isPresent.ToString() + "' WHERE zipName = '" + zipFile + "'");
        }

        //Create a list with all games from db
        public ObservableCollection<Game> fillGameList()
        {
            ObservableCollection<Game> gameList = new ObservableCollection<Game>();
            bool inMameBox;
            this.dbConn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM games", this.dbConn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr["inMamebox"].ToString() == "1")
                {
                    inMameBox = true;
                }
                else
                {
                    inMameBox = false;
                }
                gameList.Add(new Game(rdr["zipName"].ToString(), rdr["gameName"].ToString(), inMameBox));
            }
            this.dbConn.Close();
            return gameList;
        }
    }
}
