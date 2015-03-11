using System.Windows;
using CredentialChanger.ViewModel;

namespace CredentialChanger.View
{
    /// <summary>
    ///     Interaction logic for AddUserDialog.xaml
    /// </summary>
    public partial class AddUserDialog
    {
        public AddUserDialog()
        {
            InitializeComponent();
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (AddDialogModelView) AddUserGrid.DataContext;
            if (viewModel.Apply(MyPasswordBox))
            {
                Close();
                return;
            }

            MessageBox.Show("Adding Failed");
        }
    }
}