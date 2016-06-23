using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHub.ComponentModel;

namespace MyHub.ViewModels
{
    public abstract class ViewModelBase : ObservableObjectBase
    {
        /// <summary>
        /// 在具体的ViewModel中调用相关服务以获取相关数据
        /// </summary>
        /// <returns></returns>
        public virtual Task LoadState()
        {
            return Task.CompletedTask;
        }
    }
}
