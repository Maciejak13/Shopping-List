using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace Shopping_List
{
    internal class ListDatabaseService
    {
        public ListDatabaseService()
        {
            InitializeListDB();
        }

       
        public async static void InitializeListDB() //inicjowanie bazy danych
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("shopping_List.db", CreationCollisionOption.OpenIfExists);
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
               
                string initCMD = "CREATE TABLE IF NOT EXISTS " +
                    "shopping_lists (list_Id INT PRIMARY KEY, " +
                    "list_Name NVARCHAR(30) NOT NULL, " +
                    "unique(list_Name));";
                
                SqliteCommand CMDCreateTable = new SqliteCommand(initCMD, con);
                CMDCreateTable.ExecuteReader();
                con.Close();
            }
        }

        public static async void CreateBuyedTable(string nameOfList) //tworzenie tabeli kupionych zgodnie z nazwą listy.
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("shopping_List.db", CreationCollisionOption.OpenIfExists);
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                string initCMD = "CREATE TABLE IF NOT EXISTS " +
                    nameOfList + "_buyed " + "(product_Id INT PRIMARY KEY, " +
                    "product_Name NVARCHAR(30) NOT NULL, " +
                    "product_Unit NVARCHAR(10) NOT NULL, " +
                    "product_Amount REAL NOT NULL);";
                SqliteCommand CMDCreateTable = new SqliteCommand(initCMD, con);
                CMDCreateTable.ExecuteReader();
                con.Close();
            }
        }

        public static async void CreateNotBuyedTable(string nameOfList) //tworzenie tabeli nie kupionych zgodnie z nazwą listy.
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("shopping_List.db", CreationCollisionOption.OpenIfExists);
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                string initCMD = "CREATE TABLE IF NOT EXISTS " +
                    nameOfList + "_notbuyed " + "(product_Id INT PRIMARY KEY, " +
                    "product_Name NVARCHAR(30) NOT NULL, " +
                    "product_Unit NVARCHAR(10) NOT NULL, " +
                    "product_Amount REAL NOT NULL)";
                SqliteCommand CMDCreateTable = new SqliteCommand(initCMD, con);
                CMDCreateTable.ExecuteReader();
                con.Close();
            }
        }

        public class listDetails 
        {
            public string item_Name { get; set; }
            public listDetails(String ListName)
            {
                item_Name = ListName;
            }
        }

        public static void addList(String listName) //dodaj nową listę
        {
            
                string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

                using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
                {
                    con.Open();

                    SqliteCommand CMD_Insert = new SqliteCommand();
                    CMD_Insert.Connection = con;
                    CMD_Insert.CommandText = "INSERT INTO shopping_lists VALUES (@next_Id, @listName);";
                    CMD_Insert.Parameters.AddWithValue("@next_Id", getNextListId());
                    CMD_Insert.Parameters.AddWithValue("@listName", listName);
                    

                    CMD_Insert.ExecuteReader();
                    con.Close();
                }

            CreateBuyedTable(listName);
            CreateNotBuyedTable(listName);

        }


        public static void update_All_Elements_Over_Selected(int listID) //aktualizuj id list ponad wybraną w celu zachowania ciągłości id
        {

            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "UPDATE shopping_lists SET list_Id = list_Id - 1 WHERE list_Id > " + listID + "; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }

        }

        public static void removeList(int listID) //usuń listę.
        {
            removeListBuyed(foundSelectedListName(listID));
            removeListNotBuyed(foundSelectedListName(listID));
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DELETE FROM shopping_lists WHERE list_Id = '" + listID + "';";
                CMD_Insert.ExecuteReader();
                con.Close();
            }  
            update_All_Elements_Over_Selected(listID);
        }
        public static void removeListBuyed(string listName) //usun tabelke kupionych.
        {
            
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DROP TABLE "+listName+"_buyed;";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
   
        }

        public static void removeListNotBuyed(string listName) //usun tabelke nie kupionych.
        {
            
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DROP TABLE " +listName+ "_notbuyed;";
                CMD_Insert.ExecuteReader();
                con.Close();
            }

        }

        public static int getNextListId() //pobierz następne ID listy.
        {
            int LastRowID = 0;
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(list_Id) FROM shopping_lists";
                CMD_Insert.ExecuteScalar();

                object result = CMD_Insert.ExecuteScalar();
                if (result != System.DBNull.Value)
                {
                    Int64 LastRowID64 = (Int64)CMD_Insert.ExecuteScalar();
                    LastRowID = (int)LastRowID64 + 1;
                }
                else
                {
                    LastRowID = 1;
                }
                con.Close();
            }
            return LastRowID;
        }

        public static string foundSelectedListName(int listNumber) //znajdź listę po podanej nazwie.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select list_Name FROM shopping_lists WHERE list_Id = "+listNumber+" ;";
                CMD_Insert.ExecuteScalar();

                object result = CMD_Insert.ExecuteScalar();
                if (result != System.DBNull.Value)
                {
                    return (string)result;
                }
                else
                {
                    return "brak";
                }
            }
        }

        public static bool checkElementExits(string listName) //sprawdź czy dana lista o podanej nazwie istnieje.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select list_Name FROM shopping_lists WHERE list_Name = " + listName + " ;";
                try
                {
                    CMD_Insert.ExecuteScalar();
                    return true;
                }
                catch
                {
                    return false;
                }
                
            }
        }

        public static bool checkIsAnyElementInListDatabase() //sprawdz czy są jakiekolwiek rekordy w tabeli.
        {
            
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(list_Id) FROM shopping_lists";
                CMD_Insert.ExecuteScalar();

                object result = CMD_Insert.ExecuteScalar();
                if (result == System.DBNull.Value)
                {
                    con.Close();
                    return false;
                }
                else
                {
                    con.Close();
                    return true;
                }
      
            }
        }

        public static List<listDetails> GetRecords() // wyswietlanie danych pobranych z bazy
        {
            List<listDetails> shoppingList = new List<listDetails>();
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                String selectCMD = "SELECT list_Name FROM shopping_lists";
                SqliteCommand cmd_getRec = new SqliteCommand(selectCMD, con);

                SqliteDataReader reader = cmd_getRec.ExecuteReader();

                while (reader.Read())
                {
                    shoppingList.Add(new listDetails(reader.GetString(0)));
                }

                con.Close();



            }
            return shoppingList;
        }

       


    }
}
