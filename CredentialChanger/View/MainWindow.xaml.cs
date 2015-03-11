using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CredentialChanger.Model;
using CredentialChanger.ViewModel;
using CredentialManagement;

namespace CredentialChanger.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnHyperlinkClick(object sender, RoutedEventArgs e)
        {
            Uri destination = ((Hyperlink) e.OriginalSource).NavigateUri;
            try
            {
                Process.Start(destination.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Failed to open uri: " + destination);
                //Do nothing fail silently
            }
        }

        private void Context_Delete(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem) sender;

            //Get the ContextMenu to which the menuItem belongs
            var contextMenu = (ContextMenu) menuItem.Parent;

            //Find the placementTarget
            var item = (DataGrid) contextMenu.PlacementTarget;

            if (item.SelectedCells == null || item.SelectedCells.Count == 0)
            {
                return;
            }
            var modelView = (CredentialModelView) CredentialsGrid.DataContext;
            var set = new CredentialSet();
            set.Load();
            foreach (DataGridCellInfo selectedcell in item.SelectedCells)
            {
                var credential = selectedcell.Item as CustomCredentialItem;
                if (credential == null) continue;
                List<Credential> userCredentials = set.FindAll(setItem => modelView.User.Username == setItem.Username);
                Credential foundtoDelete = userCredentials.Find(credential.IsSame);
                foundtoDelete.Delete();
            }
            modelView.RefreshUsers();
        }

        private void Context_Add(object sender, RoutedEventArgs e)
        {
            var adduser = new AddUserDialog();
            adduser.ShowDialog();
            var modelView = (CredentialModelView) CredentialsGrid.DataContext;
            modelView.RefreshUsers();
        }
    }
}