using System;
using System.Collections.Generic;
using System.Diagnostics;
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


namespace Shopping_List
{
    public sealed partial class ListOfShoppingLists_Page : Page
    {
        public ListOfShoppingLists_Page()
        {
            this.InitializeComponent();
            listView.ItemsSource = ListDatabaseService.GetRecords();
            checkIsAnyElementInDatabase();
            
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e) //cofjnij na poprzednia strone.
        {
            Frame.GoBack();
        }

        private async void btn_App_Exit_Click(object sender, RoutedEventArgs e) //wyjscie z aplikacji.
        {
            ContentDialog confirmExit = new ContentDialog()
            {
                Title = "Czy na pewno chcesz zakończyć?",
                PrimaryButtonText = "Nie",
                SecondaryButtonText = "Tak",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await confirmExit.ShowAsync();
            if (result == ContentDialogResult.Secondary)
                Environment.Exit(0);
        }

        private async void btn_Add_New_List_Click(object sender, RoutedEventArgs e) //dodaj nową listę zakupów.
        {
            if (txt_List_Name.Text != "") //sprawdz czy pole nie jest puste.
            {
                try //spróboj dodać liste
                {
                    ListDatabaseService.addList(txt_List_Name.Text);
                    checkIsAnyElementInDatabase();
                    txt_List_Name.Text = "";
                    listView.ItemsSource = ListDatabaseService.GetRecords();
                }
                catch //w razie błędu informacja ze takowa juz istnieje.
                {
                    ContentDialog alreadyExits = new ContentDialog()
                    {
                        Title = "Hej! ",
                        Content = "Taka nazwa już istnieje!",
                        PrimaryButtonText = "Ok!",
                        DefaultButton = ContentDialogButton.Primary
                    };

                    var result = await alreadyExits.ShowAsync();
                    txt_List_Name.Text = "";
                }  
            }
            else
            {
                ContentDialog confirmBlankBox = new ContentDialog()
                {
                    Title = "Hej! Lista musi mieć nazwę, ",
                    Content = "sprawdź czy poprawnie ją wypełniłeś",
                    PrimaryButtonText = "Ok!",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmBlankBox.ShowAsync();
                
            }
        }

        private async void btn_Delete_Selected_List_Click(object sender, RoutedEventArgs e) //usuń wybraną listę zakupów.
        {
            
            if (listView.SelectedIndex + 1 > 0) //sprawdz czy jakikolwiek element wybrany.
            {
                ContentDialog confirmDel = new ContentDialog() //potwierdzenie usuwania listy.
                {
                    Title = "Hej!",
                    Content = " Czy na pewno chcesz usunąć listę?",
                    PrimaryButtonText = "Tak!",
                    SecondaryButtonText = "Nie!",
                    DefaultButton = ContentDialogButton.Secondary
                };

                var result = await confirmDel.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ListDatabaseService.removeList(listView.SelectedIndex + 1);
                    checkIsAnyElementInDatabase();
                    listView.ItemsSource = ListDatabaseService.GetRecords();
                }
            }
            else //jezeli nic nie wybrano wysiwetl komunikat.
            {
                ContentDialog confirmBlankDeleted = new ContentDialog()
                {
                    Title = "Hej!",
                    Content = " Musisz zaznaczyć listę, aby ją usunąć!",
                    PrimaryButtonText = "Ok!",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmBlankDeleted.ShowAsync();
            }
                        
        }

        public void checkIsAnyElementInDatabase() //sprawdż czy jakikolwiek rekord znajduje się w bazie.
        {
            bool resultOfCheck = ListDatabaseService.checkIsAnyElementInListDatabase();
            if (resultOfCheck)
            {
                txt_NoElementsInList.Visibility = Visibility.Collapsed; // jeżeli są to ukryj komunikat o braku elementów.
            }
            else
            {
                txt_NoElementsInList.Visibility = Visibility.Visible; //pokaz komunikat o braku elementów.
            }
        }

        private async void btn_Edit_Selected_List_Click(object sender, RoutedEventArgs e) // edytuj wybraną listę.
        {
            if (listView.SelectedIndex + 1 > 0) //jezeli jakikolwiek element wybrany to wykonaj polecenia.
            {
                ApplicationData.Current.LocalSettings.Values["choosenListInt"] = listView.SelectedIndex + 1;
                ApplicationData.Current.LocalSettings.Values["choosenListToEdit"] = ListDatabaseService.foundSelectedListName((int)ApplicationData.Current.LocalSettings.Values["choosenListInt"]);
                this.Frame.Navigate(typeof(ListOfProducts_Page));
                

            }
            else //jezeli nic nie wybrano wystwiel komunikat.
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

        private void btn_GoToHelp_Click(object sender, RoutedEventArgs e) //przejdź do strony z pomocą.
        {
            this.Frame.Navigate(typeof(Help_Page_Lists)); //powrót do konkretnej strony
        }
    }
}
