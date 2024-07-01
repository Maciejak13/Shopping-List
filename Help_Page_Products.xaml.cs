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
    public sealed partial class Help_Page_Products : Page
    {
        public Help_Page_Products()
        {
            this.InitializeComponent();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void btn_App_Exit_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmExit = new ContentDialog()
            {
                Title = "Czy na pewno chcesz zakończyć?",
                PrimaryButtonText = "Nie",
                SecondaryButtonText = "Tak",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await confirmExit.ShowAsync();
            if (result == ContentDialogResult.Secondary) //jeżeli chce wyjść to zamknij aplikacje.
                Environment.Exit(0);
        }

        private void btn_BackToList_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
