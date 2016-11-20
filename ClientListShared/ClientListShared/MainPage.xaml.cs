using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace ClientList
{
    public partial class MainPage : ContentPage
    {
        Db manager;
        MobileServiceUser User;
        public MainPage()
        {
            this.Title = "Client Book";
            InitializeComponent();
            manager = Db.DefaultManager;
            if(manager.IsOfflineEnabled)
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    HeightRequest = 30
                };
            }
            var tbi = new ToolbarItem() { Text = "New" };
            var backup = new ToolbarItem() { Text = "Backup Data" };
            backup.Clicked += async (a, b) =>
            {
                await Db.DefaultManager.GetClientsAsync(true);
            };
            tbi.Clicked += (a, b) =>
            {
                this.Navigation.PushAsync(new NewClient(null));
            };
            this.ToolbarItems.Add(backup);
            this.ToolbarItems.Add(tbi);
            var dataTemplate = new DataTemplate(typeof(TextCell));
            dataTemplate.SetBinding(TextCell.TextProperty, "Name");
            dataTemplate.SetBinding(TextCell.DetailProperty, "PhoneNumber");
            this.lstView.ItemsSource = Db.Clients;
            this.lstView.ItemTemplate = dataTemplate;
            this.lstView.ItemSelected += (item, d) =>
            {
                var idt = (ListView)item;
                if(idt.SelectedItem == null) return;
                this.Navigation.PushAsync(new NewClient((Client)idt.SelectedItem));
            };
        Db.DefaultManager.GetClientsAsync(true);
        }
        protected override void OnAppearing()
        {
            this.lstView.SelectedItem = null;
        }

    }
}
