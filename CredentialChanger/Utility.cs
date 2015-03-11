using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CredentialChanger.Model;
using CredentialManagement;
using Microsoft.Practices.Prism;

namespace CredentialChanger
{
    internal class Utility
    {
        public static void RefreshUsers(ObservableCollection<CredentialUser> users)
        {
            var set = new CredentialSet();
            set.Load();
            List<CredentialUser> newUsers = MapCredentials(set);
            foreach (CredentialUser oldUser in users)
            {
                CredentialUser foundUser = newUsers.Find(item => item.Username == oldUser.Username);
                if (foundUser != null)
                    foundUser.UpdateCheckbox(oldUser.Credentials.ToList());
            }
            users.Clear();
            users.AddRange(newUsers);
        }

        private static List<CredentialUser> MapCredentials(IEnumerable<Credential> set)
        {
            var newUsers = new List<CredentialUser>();
            foreach (Credential setItem in set)
            {
                var newUser = new CredentialUser(setItem);
                CredentialUser foundUser = newUsers.Find(item => item.Username == newUser.Username);
                if (foundUser != null)
                {
                    foundUser.Credentials.AddRange(newUser.Credentials);
                    continue;
                }
                newUsers.Add(newUser);
            }
            return newUsers;
        }
    }
}