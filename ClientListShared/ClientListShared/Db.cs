/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;


namespace ClientList
{
    public class Db
    {
        static Db defaultInstance = new Db();
        MobileServiceClient client;
       
        IMobileServiceSyncTable<Client> todoTable;
        public static string ClientUrl {get{ throw new NotImplementedException(); }}
        public static ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>() { };
        private Db()
        {
            this.client = new MobileServiceClient(ClientUrl);

            var store = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, "localstore1.db"));
            store.DefineTable<Client>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<Client>();


        }
        public static Db DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return todoTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Client>; }
        }

        public async Task<ObservableCollection<Client>> GetClientsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                IEnumerable<Client> items = await todoTable
                    .ToEnumerableAsync();
                Clients.Clear();
                foreach(var item in items.OrderBy(a=>a.FirstName))
                {
                    if(item.Hide) continue;
                    Clients.Add(item);
                }
                return new ObservableCollection<Client>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveTaskAsync(Client item)
        {
            if (item.Id == null)
            {
                await todoTable.InsertAsync(item);
                await this.GetClientsAsync(true);
            }
            else
            {
                await todoTable.UpdateAsync(item);
                await this.GetClientsAsync(true);
            }
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allClients",
                    this.todoTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        //await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        //await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
    }
}
