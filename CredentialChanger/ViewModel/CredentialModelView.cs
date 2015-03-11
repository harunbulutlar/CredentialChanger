using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CredentialChanger.Model;
using CredentialManagement;
using Microsoft.Practices.Prism.Commands;
using PostSharp.Patterns.Model;

namespace CredentialChanger.ViewModel
{
    [NotifyPropertyChanged]
    internal class CredentialModelView
    {
        public CredentialModelView()
        {
            Users = new ObservableCollection<CredentialUser>();
            RefreshCommand = new DelegateCommand(RefreshUsers);
            SaveCommand = new DelegateCommand<PasswordBox>(SaveObject);
            RefreshUsers();
        }

        public ObservableCollection<CredentialUser> Users { get; set; }
        public CredentialUser User { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public void RefreshUsers()
        {
            string oldUserName = User != null ? User.Username : null;
            User = null;
            Utility.RefreshUsers(Users);

            if (!string.IsNullOrEmpty(oldUserName))
            {
                User = Users.ToList().Find(user => user.Username == oldUserName);
                return;
            }
            User = Users.ElementAtOrDefault(0);
        }


        private void SaveObject(PasswordBox parameter)
        {
            if (parameter.SecurePassword.Length == 0)
            {
                return;
            }
            var set = new CredentialSet();
            set.Load();

            foreach (Credential item in User.GetCredentials(set))
            {
                item.SecurePassword = parameter.SecurePassword;
                item.Save();
            }
            RefreshUsers();
        }
    }
}