using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Shopping_List.ListDatabaseService;

namespace Shopping_List
{
    public sealed partial class ListOfProducts_Page : Page
    {
        public ListOfProducts_Page()
        {
            this.InitializeComponent();
            try //spróboj pobrać elementy kupione i nie kupione do listView
            {
                listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                listViewBuyed.ItemsSource = ProductDatabaseService.GetRecords_Buyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                checkIsAnyElementInDatabase_NotBuyed();
                choosenListText();
            }
            catch //jeżeli takich elementów nie ma, to wybrana lista jako BRAK, pokaż komunikat o braku elementów.
            {
                txtChoosenList.Text = "brak";
                txt_NoElementsInList.Visibility = Visibility.Visible;
            }
            addUnitToCombo(); //dodaje elementy do comboboxa.
            listView.ItemsSource = ListDatabaseService.GetRecords(); //wczytaj nazwy list zakupów na listView
            combo_Unit.SelectedIndex = 0; //wybierz pierwszy element w combo.
            btn_ShowBuyed.Visibility = Visibility.Visible;
            btn_ShowNotBuyed.Visibility = Visibility.Collapsed;
        }

        private void addUnitToCombo() // dodaj elementy do comboboxa. 
        {
            combo_Unit.Items.Add("kg.");
            combo_Unit.Items.Add("g.");
            combo_Unit.Items.Add("szt.");
            combo_Unit.Items.Add("dag.");
            combo_Unit.Items.Add("m.");
            combo_Unit.Items.Add("t.");
            combo_Unit.Items.Add("l.");

        }
     
        private void choosenListText() // wpisz nazwę wybranej listy w TextBox.
        {
            if (ApplicationData.Current.LocalSettings.Values["choosenListInt"] != null) //sprawdź czy numer listy !null.
            {
                txtChoosenList.Text = ListDatabaseService.foundSelectedListName((int)ApplicationData.Current.LocalSettings.Values["choosenListInt"]); //zmien tekst na nazwę wybranej listy.
            }
        }
        
        private async void btn_App_Exit_Click(object sender, RoutedEventArgs e) //przycisk od wyłączania aplikacji.
        {
            ContentDialog confirmExit = new ContentDialog() // potwierdzenie wyjścia.
            {
                Title = "Czy na pewno chcesz zakończyć?",
                PrimaryButtonText = "Nie",
                SecondaryButtonText = "Tak",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await confirmExit.ShowAsync();
            if (result == ContentDialogResult.Secondary) //jeżeli chce wyjśc to zamknij aplikacje.
                Environment.Exit(0);
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e) //przycisk od cofania strony.
        {
            Frame.GoBack(); //confij stronę.
        }

        private async void b_Click(object sender, RoutedEventArgs e) //przycisk od wyboru listy do edycji.
        {
            if(ListDatabaseService.checkIsAnyElementInListDatabase()) // jeżeli jest jakikolwiek rekord w tabelce z listami, to pokaż dynamiczne elementy i ukryj nie potrzebne.
            {
                btn_ShowBuyed.Visibility = Visibility.Collapsed;
                btn_ShowNotBuyed.Visibility = Visibility.Collapsed;
                txt_NoElementsInList.Visibility = Visibility.Collapsed;
                txt_NoBuyedElements.Visibility = Visibility.Collapsed;
                listViewProducts.Visibility = Visibility.Collapsed;
                listView.Visibility = Visibility.Visible;
                txt_ChoseListToEdit.Visibility = Visibility.Visible;
                listViewBuyed.Visibility = Visibility.Collapsed;
                sp_ChoseList.Visibility = Visibility.Visible;
                b.Visibility = Visibility.Collapsed;
                b2.Visibility = Visibility.Visible;
            }
            else //jezeli brak elementów w tabelce z listami, to pokaż komunikat.
            {
                ContentDialog noShopLists = new ContentDialog()
                {
                    Title = "Hej!",
                    Content = "Nie dodałeś żadnej listy!",
                    PrimaryButtonText = "Ok!",
                    SecondaryButtonText = "Chce ją dodać!",
                    DefaultButton = ContentDialogButton.Primary
                    
                };
                var result = await noShopLists.ShowAsync();
                if(result == ContentDialogResult.Secondary) // jeżeli chce dodać liste przekieruj do strony z dodawaniem list. 
                    this.Frame.Navigate(typeof(ListOfShoppingLists_Page));
            }
        }

        private async void b2_Click(object sender, RoutedEventArgs e) // potwierdź wybór listy zakupów. 
        {
            if (listView.SelectedIndex + 1 > 0) //jeżeli jest poprawnie zaznaczona lista
            {
                
                ApplicationData.Current.LocalSettings.Values["choosenListInt"] = listView.SelectedIndex + 1; //zmień numer wybranej listy. 
                ApplicationData.Current.LocalSettings.Values["choosenListToEdit"] = ListDatabaseService.foundSelectedListName((int)ApplicationData.Current.LocalSettings.Values["choosenListInt"]); //zmień nazwę wybranej listy.
                // ukryj nie potrzebnne elementy i wyświetl menu od wyboru listy do edycji.
                listViewProducts.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Collapsed;
                txt_ChoseListToEdit.Visibility = Visibility.Collapsed;
                sp_ChoseList.Visibility = Visibility.Collapsed;
                b.Visibility = Visibility.Visible;
                b2.Visibility = Visibility.Collapsed;
                listViewBuyed.ItemsSource = ProductDatabaseService.GetRecords_Buyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);

                if (listViewProducts.Visibility == Visibility.Visible) // jeżeli lista nie kupionych jest widoczna to wczytaj nie kupione.
                {
                    checkIsAnyElementInDatabase_NotBuyed();
                    txt_ProductsToBuy.Visibility = Visibility.Visible;
                    txt_ProductsBuyed.Visibility= Visibility.Collapsed;
                }
                else if (listViewBuyed.Visibility == Visibility.Visible) // jeżeli lista kupionych jest widoczna to wczytaj kupione.
                {
                    checkIsAnyElementInDatabase_Buyed();
                    txt_ProductsToBuy.Visibility = Visibility.Collapsed;
                    txt_ProductsBuyed.Visibility = Visibility.Visible;
                }

                btn_ShowBuyed.Visibility = Visibility.Visible;
                btn_ShowNotBuyed.Visibility = Visibility.Collapsed;
                choosenListText();
                InitializeComponent(); //wczytaj ponownie stronę. 
                }
            else //jeżeli nic nie jest zaznaczone wyświetl komunikat o błędzie. 
            {
                ContentDialog confirmBlankDeleted = new ContentDialog()
                {
                    Title = "Hej!",
                    Content = " Musisz zaznaczyć listę, aby ją edytować!",
                    PrimaryButtonText = "Ok!",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmBlankDeleted.ShowAsync();
            }    
        }

        private async void btn_Add_New_Product_Click(object sender, RoutedEventArgs e) //dodaj nowy produkt do listy.
        {
            if (String.IsNullOrWhiteSpace(txt_Product_Name.Text) || String.IsNullOrWhiteSpace(combo_Unit.SelectedItem.ToString()) || String.IsNullOrWhiteSpace(txt_Product_Amount.Text)) //sprawdź czy wszystko zostało wypełnione.
            {
                //jeżeli nie wypełnione to wyświetl komunikat. 
                ContentDialog confirmBlankBox = new ContentDialog()
                {
                    Title = "Hej!",
                    Content = "sprawdź czy poprawnie wypełniłeś dane",
                    PrimaryButtonText = "Ok!",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmBlankBox.ShowAsync();
            }
            else //jeżeli wszystko wypełnione.
            {
                try //spróboj wykonac polecenie sql dodawania do bazy.
                {
                    ProductDatabaseService.addProduct(txt_Product_Name.Text, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"], combo_Unit.SelectedItem.ToString(), txt_Product_Amount.Text);
                    checkIsAnyElementInDatabase_NotBuyed();
                    txt_Product_Name.Text = "";
                    listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                }
                catch //jeżeli błąd (nie ma takiej tabeli), wyświetl komunikat o błędzie. 
                {
                    ContentDialog noListSelected = new ContentDialog()
                    {
                        Title = "Hej!",
                        Content = "sprawdź czy poprawnie wybrałeś liste.",
                        PrimaryButtonText = "Ok!",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    var result = await noListSelected.ShowAsync();
                }
                                
            }
        }

        public void checkIsAnyElementInDatabase_NotBuyed() //funkcja sprawdzająca czy jest jakikolwiek element w nie kupionych produktach. 
        {
            bool resultOfCheck = ProductDatabaseService.checkIsAnyElementInDatabase_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
            if (resultOfCheck) //jeżeli jest to nie pokazuj komunikatu o braku elementów. 
            {
                scrollNotBuyed.Visibility = Visibility.Visible;
                txt_NoElementsInList.Visibility = Visibility.Collapsed;        
            }
            else //jeżeli nie ma to pokaż komunikat o braku elementów w bazie.
            {  
                txt_NoElementsInList.Visibility = Visibility.Visible;
            }
        }
        
        public void checkIsAnyElementInDatabase_Buyed() //funkcja sprawdzająca czy jest jakikolwiek element w kupionych produktach. 
        {  
            bool resultOfCheck_Buyed = ProductDatabaseService.checkIsAnyElementInDatabase_Buyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
            if (resultOfCheck_Buyed) //jeżeli jest to nie pokazuj komunikatu o braku elementów. 
            {
                txt_NoBuyedElements.Visibility = Visibility.Collapsed;
            }
            else //jeżeli nie ma to pokaż komunikat o braku elementów w bazie.
            {
                txt_NoBuyedElements.Visibility = Visibility.Visible;
            }
        }
        

        private async void btn_Delete_Selected_Product_Click(object sender, RoutedEventArgs e) //usuń produkt z listy. 
        {
            if (listViewProducts.SelectedIndex + 1 > 0) //sprawdź czy jest cokolwiek zaznaczone. 
            {
                ProductDatabaseService.removeProduct(listViewProducts.SelectedIndex + 1, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //wykonaj usuwanie produktu z bazy. 
                checkIsAnyElementInDatabase_NotBuyed(); //funkcja sprawdzania czy jest jakiś element w bazie. 
                listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //wczytaj ponownie produkty.
            }
            else // jeżeli nie jest zaznaczone. 
            {
                if(listViewBuyed.SelectedIndex + 1 > 0) //jeżeli zaznaczony jest kupiony element. 
                {
                    ContentDialog confirmBlankDeleted = new ContentDialog()
                    {
                        Title = "Hej!",
                        Content = " Nie możesz usunać już kupionego produktu!!",
                        PrimaryButtonText = "Ok!",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    var result = await confirmBlankDeleted.ShowAsync();
                }
                else // jeżeli nic nie jest zaznaczone.
                {
                    ContentDialog confirmBlankDeleted = new ContentDialog()
                    {
                        Title = "Hej!",
                        Content = " Musisz zaznaczyć produkt, aby go usunąć!",
                        PrimaryButtonText = "Ok!",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    var result = await confirmBlankDeleted.ShowAsync();
                }
            }
        }

        private void btn_ShowBuyed_Click(object sender, RoutedEventArgs e) //pokaz kupione elementy.
        {
            try //spróboj pobrać kupione elementy z bazy.
            {
                listViewBuyed.ItemsSource = ProductDatabaseService.GetRecords_Buyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                checkIsAnyElementInDatabase_Buyed();
            }
            catch //jeżeli ich brak wyświetl komunikat.
            {
                txt_NoBuyedElements.Visibility = Visibility.Visible;
            }
            //dynamiczne elementy zmieniające się zgodnie z tym co chcemy wyświetlać.
            btn_ShowBuyed.Visibility = Visibility.Collapsed;
            btn_ShowNotBuyed.Visibility = Visibility.Visible;
            txt_NoElementsInList.Visibility = Visibility.Collapsed;
            listViewProducts.Visibility = Visibility.Collapsed;
            listViewBuyed.Visibility = Visibility.Visible;
            txt_ProductsBuyed.Visibility = Visibility.Visible;
            txt_ProductsToBuy.Visibility = Visibility.Collapsed;    
            btn_Change_Buy_Status_To_NotBuyed.Visibility = Visibility.Visible;
            btn_Change_Buy_Status_To_Buyed.Visibility = Visibility.Collapsed;
        }

        private async void btn_Change_Buy_Status_To_Buyed_Click(object sender, RoutedEventArgs e)
        {
            // zmień status przedmiotu z nie kupionego, na kupiony
            if (listViewProducts.SelectedIndex + 1 > 0)
            {
                ProductDatabaseService.update_Status_NotBuyed_ToBuyed(listViewProducts.SelectedIndex + 1, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                checkIsAnyElementInDatabase_NotBuyed();
                listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
            }
            else //wyświetl komunikat o nie zaznaczonym produkcie.
            {
                    ContentDialog confirmBlankChangeStatus = new ContentDialog()
                    {
                        Title = "Hej!",
                        Content = " Musisz zaznaczyć produkt, aby go kupić!",
                        PrimaryButtonText = "Ok!",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    var result = await confirmBlankChangeStatus.ShowAsync();
            }
        }

        private async void btn_Change_Buy_Status_To_NotBuyed_Click(object sender, RoutedEventArgs e)  //zmień status przedmiotu z kupionego, na nie kupiony.
        {    
            if (listViewBuyed.SelectedIndex + 1 > 0) //sprawdź czy jakiś element jest zaznaczony na liście zakupów. 
            {
                ProductDatabaseService.update_Status_Buyed_ToNotBuyed(listViewBuyed.SelectedIndex + 1, (string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //wykonaj funkcje zmiany statusu. 
                checkIsAnyElementInDatabase_Buyed();

                listViewBuyed.ItemsSource = ProductDatabaseService.GetRecords_Buyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]); //pobierz elementy na liste.
                //dynamiczne elementy zmieniające się zgodnie z tym co chcemy wyświetlać.
                btn_ShowBuyed.Visibility = Visibility.Collapsed;
                btn_ShowNotBuyed.Visibility = Visibility.Visible;
                txt_NoElementsInList.Visibility = Visibility.Collapsed;
                listViewProducts.Visibility = Visibility.Collapsed;
                listViewBuyed.Visibility = Visibility.Visible;
                btn_Change_Buy_Status_To_NotBuyed.Visibility = Visibility.Visible;
                btn_Change_Buy_Status_To_Buyed.Visibility = Visibility.Collapsed;
            }
            else //jeżeli brak zaznaczenia wyświetl komunikat. 
            {
                ContentDialog confirmBlankChangeStatus = new ContentDialog()
                {
                    Title = "Hej!",
                    Content = " Musisz zaznaczyć produkt by przenieść go do nie kupionych!",
                    PrimaryButtonText = "Ok!",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmBlankChangeStatus.ShowAsync();
            }
        }
        private void btn_ShowNotBuyed_Click(object sender, RoutedEventArgs e) //spróboj pobrać dane z bazy, jeżeli ich nie ma to wyświetl komunikat, by je dodać.
        {
            try
            {
                listViewProducts.ItemsSource = ProductDatabaseService.GetRecords_NotBuyed((string)ApplicationData.Current.LocalSettings.Values["choosenListToEdit"]);
                checkIsAnyElementInDatabase_NotBuyed();
            }
            catch //jeżeli polecenie w bazie się nie powiodło (brak elementów w tej liście), wyświetl komunikat o braku elementów. 
            {
                txt_NoElementsInList.Visibility = Visibility.Visible;
            }
            //dynamiczne elementy zmieniające się zgodnie z tym co chcemy wyświetlać.
            txt_NoBuyedElements.Visibility = Visibility.Collapsed;
            btn_ShowBuyed.Visibility = Visibility.Visible;
            btn_ShowNotBuyed.Visibility = Visibility.Collapsed;
            listViewProducts.Visibility = Visibility.Visible;
            txt_ProductsBuyed.Visibility = Visibility.Collapsed;
            txt_ProductsToBuy.Visibility = Visibility.Visible;
            listViewBuyed.Visibility = Visibility.Collapsed;
            btn_Change_Buy_Status_To_NotBuyed.Visibility = Visibility.Collapsed;
            btn_Change_Buy_Status_To_Buyed.Visibility = Visibility.Visible;
           
        }

        private void btn_GoToHelp_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Help_Page_Products)); //nawigacja do konkretnej strony
        }
    } 
}