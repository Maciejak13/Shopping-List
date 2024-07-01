using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace Shopping_List
{
    internal class ProductDatabaseService
    {
        public ProductDatabaseService()
        {
           
            InitializeHashProduct();
        }
        public async static void InitializeHashProduct() //inicjalizacja tabeli z listami zakupów.
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("shopping_List.db", CreationCollisionOption.OpenIfExists); //otwórz bazę jeżeli istnieje.
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db"); //wskaż scieżkę do bazy.
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open(); // otwórz połączenie z bazą.
                    string initCMD = 
                    "CREATE TABLE IF NOT EXISTS " +
                    "product_hash (product_Id INT PRIMARY KEY, " +
                    "product_Name NVARCHAR(30) NOT NULL, " +
                    "product_Unit NVARCHAR(10) NOT NULL, " +
                    "product_Amount REAL NOT NULL);";  // polecenie sql
                SqliteCommand CMDCreateTable = new SqliteCommand(initCMD, con); //utwórz nowe polecenie.
                CMDCreateTable.ExecuteReader(); //wykonaj polecenie.
                con.Close(); //zamknij połączenie z bazą.
            }
        }

        public static bool checkIsAnyElementInDatabase_NotBuyed(string listName) //sprawdź czy jest jakiś element w nie kupionych.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(product_Id) FROM "+listName+"_notbuyed; ";
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

        public static bool checkIsAnyElementInDatabase_Buyed(string listName) //sprawdź czy jest jakiś element w kupionych.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(product_Id) FROM " + listName + "_buyed; ";
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

        public class productDetails
        {
            public string item_Name { get; set; }
            public string item_Amount { get; set; }
            public string item_Unit { get; set; }
            public productDetails(String ProductName, string Amount, string Unit)
            {
                item_Name = ProductName;
                item_Amount = Amount;
                item_Unit = Unit;
            }
        }

        public static List<productDetails> GetRecords_NotBuyed(string listName) //wczytaj rekordy z bazy danych nie kupione.
        {
            List<productDetails> productList = new List<productDetails>();
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                String selectCMD = "SELECT product_Name, product_Amount, product_Unit FROM " + listName + "_notbuyed";
                SqliteCommand cmd_getRec = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();
                while (reader.Read()) //przeskakuj po rekordach i dodawaj je do listy.
                {
                    productList.Add(new productDetails(reader.GetString(0), reader.GetString(1), reader.GetString(2))); //dodaj rodukty do listy.
                }
                con.Close();
            }
            return productList; //zwróć listę produktów.
        }

        public static List<productDetails> GetRecords_Buyed(string listName) //wczytaj rekordy z bazy danych kupione.
        {
            List<productDetails> productList = new List<productDetails>();
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                String selectCMD = "SELECT product_Name, product_Amount, product_Unit FROM "+listName+"_buyed";
                SqliteCommand cmd_getRec = new SqliteCommand(selectCMD, con);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();
                while (reader.Read())
                {
                    productList.Add(new productDetails(reader.GetString(0), reader.GetString(1), reader.GetString(2)));
                }
                con.Close();
            }
            return productList;
        }

        public static void addProduct(String productName, string listName, String unit, String amount) //dodaj nowy projekt do konkretnej listy.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "INSERT INTO "+listName+"_notbuyed VALUES (@next_Id, @productName, @unit, @amount);";
                CMD_Insert.Parameters.AddWithValue("@next_Id", getNextProductId(listName));
                CMD_Insert.Parameters.AddWithValue("@productName", productName);
                CMD_Insert.Parameters.AddWithValue("@unit", unit);
                CMD_Insert.Parameters.AddWithValue("@amount", amount);
                CMD_Insert.ExecuteReader();
                con.Close();
            }

        }

        public static int getNextProductId(string listName) // pobierz następne ID produktu w nie kupionych.
        {
            int LastRowID = 0;
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(product_Id) FROM "+listName+"_notbuyed;";
                CMD_Insert.ExecuteScalar();

                object result = CMD_Insert.ExecuteScalar();
                if (result != System.DBNull.Value) //jeżeli są elementy to zwróć IdOstatniegoElementu +1
                {
                    Int64 LastRowID64 = (Int64)CMD_Insert.ExecuteScalar();
                    LastRowID = (int)LastRowID64 + 1;
                }
                else // jeżeli brak elementów to zwróc ostatni element jako 1
                {
                    LastRowID = 1;
                }
                con.Close();
            }
            return LastRowID;
        }
        public static int getNextProductIdBuyed(string listName) //zwróć następne ID produktu w kupionych.
        {
            int LastRowID = 0;
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "select max(product_Id) FROM "+listName+"_buyed;";
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
        public static void update_All_Elements_Over_Selected(int productID, string listName) // zaktualizuj wszystkie ID produktu powyżej zaznaczonego (id_Produktu -1) w celu zachowania ciągłości ID
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "UPDATE "+listName+"_notbuyed SET product_Id = product_Id - 1 WHERE product_Id > " + productID + "; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }

        }

        public static void update_All_Elements_Over_Selected_Buyed(int productID, string listName)  // zaktualizuj wszystkie ID produktu powyżej zaznaczonego w kupionych (id_Produktu -1) w celu zachowania ciągłości ID
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "UPDATE "+listName+"_buyed SET product_Id = product_Id - 1 WHERE product_Id > " + productID + "; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
        }

        public static void update_Status_NotBuyed_ToBuyed(int productID, string listName) //przerzuć produkt z nie kupionych do tablicy hash.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "INSERT INTO product_hash SELECT product_Id, product_Name, product_Unit, product_Amount FROM "+listName+"_notbuyed WHERE product_Id = "+productID+"; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            removeProduct(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //usuń produkt.
            update_Product_Id_NotBuyed(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //aktualizuj Id produktów.
        }
        public static void update_Product_Id_NotBuyed(int productID, string listName) //zaktualizuj Id produktu w hash zgodnie z następnym ID w docelowej liscie (kupionych), by zachować ciągłość Id w tabeli kupionych.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "UPDATE product_hash SET product_Id = @newID WHERE product_Id = " + productID + "; ";
                CMD_Insert.Parameters.AddWithValue("@newID", getNextProductIdBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]));
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            insert_NotBuyedToBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //wykonaj fukcję wrzucenia produktu do tabeli kupionych.
        }

        public static void insert_NotBuyedToBuyed(string listName) //wrzuc element z hashu do tabeli kupionych produktów.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "INSERT INTO "+listName+"_buyed SELECT product_Id, product_Name, product_Unit, product_Amount FROM product_hash WHERE product_Id = " + getNextProductIdBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]) + "; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            clearHashTable(); //funkcja czyszcząca tabele hash.
        }

        public static void clearHashTable() //wyczyść tablicę hash.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DELETE FROM product_hash";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
        }

        public static void update_Status_Buyed_ToNotBuyed(int productID, string listName)  //przerzuć produkt z kupionych do tablicy hash.
        {

            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();

                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "INSERT INTO product_hash SELECT product_Id, product_Name, product_Unit, product_Amount FROM "+listName+"_buyed WHERE product_Id = " + productID + "; ";
                
                CMD_Insert.ExecuteReader();
                con.Close();
            }

            removeProductBuyed(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //usuń produkt.
            update_Product_Id_Buyed(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //aktualizuj ID produktu.

        }


        public static void update_Product_Id_Buyed(int productID, string listName) //zaktualizuj Id produktu w hash zgodnie z następnym ID w docelowej liscie (nie kupionych), by zachować ciągłość Id w tabeli nie kupionych.
        {

            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");

            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "UPDATE product_hash SET product_Id = @newID WHERE product_Id = " + productID + "; ";
                CMD_Insert.Parameters.AddWithValue("@newID", getNextProductId(listName));

                CMD_Insert.ExecuteReader();
                con.Close();
            }
            insert_BuyedToNotBuyed(listName); //wrzuć element do tablicy nie kupionych.
        }

        public static void insert_BuyedToNotBuyed(string listName) //wrzuc element z hashu do tabeli nie kupionych produktów.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "INSERT INTO "+listName+"_notbuyed SELECT product_Id, product_Name, product_Unit, product_Amount FROM product_hash WHERE product_Id = " + getNextProductId(listName) + "; ";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            clearHashTable(); //wyczyść tablicę hash.
        }

        public static void removeProduct(int productID, string listName) //usuń produkt z nie kupionych.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DELETE FROM "+listName+"_notbuyed WHERE product_Id = '" + productID + "';";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            update_All_Elements_Over_Selected(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //aktualizuj ID produktów nad produktem usuwanym
        }

        public static void removeProductBuyed(int productID, string listName) //usuń produkt z kupionych.
        {
            string patchToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "shopping_List.db");
            using (SqliteConnection con = new SqliteConnection($"Filename = {patchToDB}"))
            {
                con.Open();
                SqliteCommand CMD_Insert = new SqliteCommand();
                CMD_Insert.Connection = con;
                CMD_Insert.CommandText = "DELETE FROM "+listName+"_buyed WHERE product_Id = '" + productID + "';";
                CMD_Insert.ExecuteReader();
                con.Close();
            }
            update_All_Elements_Over_Selected_Buyed(productID, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //aktualizuj ID produktów nad produktem usuwanym.
        }
    }
}
