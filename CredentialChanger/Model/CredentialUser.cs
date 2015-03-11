using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CredentialManagement;
using Microsoft.Practices.Prism.Commands;
using PostSharp.Patterns.Model;

namespace CredentialChanger.Model
{
    [NotifyPropertyChanged]
    internal class CredentialUser
    {
        public CredentialUser(string username)
        {
            Username = username;
            Init();
        }

        public CredentialUser(Credential credential)
        {
            Username = credential.Username;
            Init(new CustomCredentialItem(credential));
        }

        public ICommand AllCheckCommand { get; set; }
        public string Username { get; set; }
        public bool? IsAllChecked { get; set; }
        public ObservableCollection<CustomCredentialItem> Credentials { get; set; }

        public void Init(CustomCredentialItem item = null)
        {
            Credentials = new ObservableCollection<CustomCredentialItem>();
            Credentials.CollectionChanged += CredentialsChanged;
            if (item != null)
            {
                Credentials.Add(item);
            }
            AllCheckCommand = new DelegateCommand<bool?>(AllCheck);
            IsAllChecked = true;
            AllCheck(true);
        }

        private void CredentialsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CustomCredentialItem item in e.NewItems)
                    item.PropertyChanged += CustomCredentialItemPropertyChanged;

            if (e.OldItems != null)
                foreach (CustomCredentialItem item in e.OldItems)
                    item.PropertyChanged -= CustomCredentialItemPropertyChanged;
        }

        private void CustomCredentialItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked") return;
            if (Credentials.All(x => x.IsChecked == true))
            {
                IsAllChecked = true;
            }
            else if (Credentials.All(x => x.IsChecked == false))
            {
                IsAllChecked = false;
            }
            else
            {
                IsAllChecked = null;
            }
        }

        private void AllCheck(bool? parameter)
        {
            bool allcheck = parameter != null && (bool) parameter;
            if (allcheck)
            {
                Credentials.ToList().ForEach(x => x.IsChecked = true);
            }
            else
            {
                Credentials.ToList().ForEach(x => x.IsChecked = false);
            }
        }

        public void RefreshCredentials()
        {
            var set = new CredentialSet();
            set.Load();
            IEnumerable<CustomCredentialItem> result =
                set.Where(item => item.Username == Username).Select(item => new CustomCredentialItem(item));

            Credentials.Clear();
            foreach (CustomCredentialItem item in result)
            {
                Credentials.Add(item);
            }
        }

        public IEnumerable<Credential> GetCredentials(CredentialSet set)
        {
            IEnumerable<Credential> userCredentials = set.Where(item => item.Username == Username);
            return
                Credentials.Where(item => item.IsChecked)
                    .Select(currentCred => userCredentials.First(currentCred.IsSame));
        }

        public void UpdateCheckbox(IEnumerable<CustomCredentialItem> oldCredentials)
        {
            foreach (CustomCredentialItem oldCredential in oldCredentials)
            {
                CustomCredentialItem credential = oldCredential;
                foreach (
                    CustomCredentialItem newCredential in
                        Credentials.Where(newCredential => newCredential.IsSame(credential)))
                {
                    newCredential.IsChecked = oldCredential.IsChecked;
                }
            }
        }
    }
}