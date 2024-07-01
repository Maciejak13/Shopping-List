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


namespace Shopping_List
{
    public sealed partial class Help_Page_Lists : Page
    {
        public Help_Page_Lists()
        {
            this.InitializeComponent();
        }

        private void btn_BackToList_Click(object sender, RoutedEventArgs e) //powrót do strony z listami zakupów.
        {
            Frame.GoBack();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack(); //powrót do poprzedniej strony
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

        
    }
}
