using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shopping_List
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainMenu_Page : Page
    {
        public MainMenu_Page()
        {
            this.InitializeComponent();
        }

        private async void btn_App_Exit_Click(object sender, RoutedEventArgs e) //przycisk wyłączania aplikacji.
        {
            ContentDialog confirmExit = new ContentDialog()
            {
                Title = "Czy na pewno chcesz zakończyć?",
                PrimaryButtonText = "Nie",
                SecondaryButtonText = "Tak",
                DefaultButton = ContentDialogButton.Primary
            };
            var result  = await confirmExit.ShowAsync();
            if (result == ContentDialogResult.Secondary) //jeżeli chce wyjśc to zamknij aplikacje.
                Environment.Exit(0); 
        }

        private void btn_GoToHelp_Click(object sender, RoutedEventArgs e) // przejdź do strony z pomocą. 
        {
            this.Frame.Navigate(typeof(Help_Page));
        }

        private void btn_goToShoppingLists_Click(object sender, RoutedEventArgs e) // przejdź do strony z listami zakupów. 
        {
            this.Frame.Navigate(typeof(ListOfShoppingLists_Page));
        }

       
        private void btn_goToProducts_Click_1(object sender, RoutedEventArgs e) // przejdź do strony z produktami.
        {
            this.Frame.Navigate(typeof(ListOfProducts_Page));
        }
    }
}
