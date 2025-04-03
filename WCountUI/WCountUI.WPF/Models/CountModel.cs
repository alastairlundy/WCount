using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WCountUI.WPF.Models
{
    public class CountModel : INotifyPropertyChanged
    {
        private ulong wordCount;
        private int characterCount;

        public CountModel()
        {
          

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
