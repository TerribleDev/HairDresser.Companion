using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ClientList
{
    public partial class NewClient : ContentPage
    {
        public Client Client { get; set; }
        public NewClient(Client client)
        {
            if(client == null)
            {
                Client = new Client();
            }
            else
            {
                this.Client = client;
            }
            InitializeComponent();
            this.slLayout.BindingContext = Client;
            var tbi = new ToolbarItem() { Text = "Save", Name = "Save" };
            tbi.Clicked += (obj, args) =>
            {
                Db.DefaultManager.SaveTaskAsync(Client).ConfigureAwait(false);
                this.Navigation.PopAsync();

            };
            var delItem = new ToolbarItem() { Text = "Delete" };
            delItem.Clicked += async (obj, args) =>
            {
                var alert = await DisplayAlert("delete", "Are you sure you want to delete?", "Yes", "No");
                if(alert)
                {
                    Client.Hide = true;
                    await Db.DefaultManager.SaveTaskAsync(Client);
                    this.Navigation.PopAsync();
                }
                
            };
            this.ToolbarItems.Add(tbi);
            this.ToolbarItems.Add(delItem);
            slLayout.VerticalOptions = LayoutOptions.StartAndExpand;
        }
    }
}
