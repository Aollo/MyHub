using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHub.ComponentModel
{
    /// <summary>
    /// 继承自INotifyPropertyChanged的类，封装了引发PropertyChanged事件的方法
    /// </summary>
    public class ObservableObjectBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 在属性更改时会引发的事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性已经更改
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
