using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Controls;
using CredentialChanger.Model;
using CredentialManagement;
using PostSharp.Patterns.Model;

namespace CredentialChanger.ViewModel
{
    [NotifyPropertyChanged]
    internal class AddDialogModelView
    {
        private const string c_NewUser = "<New User>";
        private readonly CredentialUser NewUserCredential = new CredentialUser(c_NewUser);
        private CredentialUser _user;

        public AddDialogModelView()
        {
            Users = new ObservableCollection<CredentialUser>();
            Utility.RefreshUsers(Users);
            Users.Insert(0, NewUserCredential);
            User = Users[0];
        }

        public ObservableCollection<CredentialUser> Users { get; set; }
        public string NewCredential { get; set; }
        public CredentialType Type { get; set; }
        public bool IsUserEnabled { get; set; }
        public string NewUser { get; set; }

        public CredentialUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (_user != null && _user.Username == c_NewUser)
                {
                    IsUserEnabled = true;
                    NewUser = "";
                    return;
                }
                if (_user != null) NewUser = _user.Username;
                IsUserEnabled = false;
            }
        }

        public bool Apply(PasswordBox password)
        {
            if (string.IsNullOrEmpty(NewUser) ||
                string.IsNullOrEmpty(NewCredential) ||
                password.SecurePassword.Length == 0) return false;

            var cred = new Credential(NewUser, ConvertToUnsecureString(password.SecurePassword), NewCredential, Type)
            {
                SecurePassword = password.SecurePassword,
                PersistanceType = PersistanceType.LocalComputer
            };

            return cred.Save();
        }

        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}