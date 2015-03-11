using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CredentialChanger.Annotations;
using CredentialManagement;
using PostSharp.Patterns.Model;

namespace CredentialChanger.Model
{
    [NotifyPropertyChanged]
    public class CustomCredentialItem : INotifyPropertyChanged
    {
        public CustomCredentialItem()
        {
        }

        public CustomCredentialItem(Credential originalCredential)
        {
            Credential = originalCredential.Target;
            Type = originalCredential.Type;
            LastChangeTime = originalCredential.LastWriteTime;
            Description = originalCredential.Description;
        }

        public bool IsChecked { get; set; }

        public string Credential { get; set; }

        public CredentialType Type { get; set; }

        public DateTime? LastChangeTime { get; set; }
        public string Description { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsSame(CustomCredentialItem compared)
        {
            return Credential == compared.Credential && Type == compared.Type;
        }

        public bool IsSame(Credential compared)
        {
            return Credential == compared.Target && Type == compared.Type;
        }
    }
}