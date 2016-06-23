using System;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using MyHub.Commands;
using MyHub.Services;

namespace MyHub.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        private bool _isWeiboLogin;
        private bool _isKaixinLogin;
        private bool _isStartUsingEnable;
        private const string _weiboSnsName = "新浪微博";
        private const string _kaixinSnsName = "开心网";
        private IAuthorizationService _authService;
        private Models.Account _account;

        public WelcomeViewModel()
        {
            _isWeiboLogin = false;
            _isKaixinLogin = false;
            _isStartUsingEnable = false;
            _authService = null;
            _account = null;

            WeiboLoginCommand = new RelayCommand(OnWeiboLogin, () => !IsWeiboLogin);
            KaixinLoginCommand = new RelayCommand(OnKaixinLogin, () => !IsKaixinLogin);
            StartUsingCommand = new RelayCommand(OnStartUsing, () => IsStartUsingEnable);
            QuickLoginCommand = new RelayCommand(OnQuickLogin);
            NormalLoginCommand = new RelayCommand(OnNormalLogin);

            Lifecycle.AppRuntimeEnvironment.Instance.PropertyChanged += UserAccount_PropertyChanged;
        }

        public RelayCommand WeiboLoginCommand { get; private set; }

        public RelayCommand KaixinLoginCommand { get; private set; }

        public RelayCommand StartUsingCommand { get; private set; }

        public RelayCommand QuickLoginCommand { get; private set; }

        public RelayCommand NormalLoginCommand { get; private set; }

        public bool IsWeiboLogin
        {
            get { return _isWeiboLogin; }
            set
            {
                if(_isWeiboLogin != value)
                {
                    _isWeiboLogin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsKaixinLogin
        {
            get { return _isKaixinLogin; }
            set
            {
                if(_isKaixinLogin != value)
                {
                    _isKaixinLogin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsStartUsingEnable
        {
            get { return _isStartUsingEnable; }
            set
            {
                if(_isStartUsingEnable != value)
                {
                    _isStartUsingEnable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Models.Account Account
        {
            get { return _account; }
            set
            {
                if(_account != value)
                {
                    _account = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override Task LoadState()
        {
            return base.LoadState();
        }

        private async void OnWeiboLogin()
        {
            await DoLogin(_weiboSnsName);
        }

        private async void OnKaixinLogin()
        {
            await DoLogin(_kaixinSnsName);
        }

        /// <summary>
        /// 执行登录的公用
        /// </summary>
        private async Task DoLogin(string snsName)
        {
            Account = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccountUnlogin(snsName);
            _authService = ServiceLocator.Current.GetInstance<IAuthorizationService>(snsName);
            if (Account != null)// 打开支持快速登陆的对话框
            {
                NotifyPropertyChanged("ShowLoginDialog");// 作用是引发事件，以让前台显示登陆对话框
            }
            else// 执行授权
            {
                await _authService.DoAuthorization();
            }
        }

        private void OnStartUsing()
        {
            var service = ServiceLocator.Current.GetInstance<ILocalDataService>();
            Models.Account[] accounts = Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccount();
            if (accounts.Length <= 0)
                return;
            foreach(Models.Account a in accounts)
            {
                a.LocalAccountId = Convert.ToInt32(service.StoreAccount(a));// 统一把信息保存到数据库
                Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccount(a);// 把本地账号ID更新，因为如果要更新本地数据库需要此ID
            }

            var content = Windows.UI.Xaml.Window.Current.Content;
            if(content is Windows.UI.Xaml.Controls.Frame)
            {
                var frame = content as Windows.UI.Xaml.Controls.Frame;
                frame.Navigate(typeof(Views.MainPage));
            }
            else
            {
                throw new ArgumentException("Window.Current.Content");
            }
        }

        /// <summary>
        /// 快速登陆，只需要修改应用程序运行时全局数据中心中的账户状态、修改数据库
        /// </summary>
        private void OnQuickLogin()
        {
            _account.isAvailable = true;
            Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccount(_account);// 添加到登录用户的列表
            Lifecycle.AppRuntimeEnvironment.Instance.RemoveUserAccountUnlogin(_account);// 从未登录用户列表中移除

            NotifyPropertyChanged("HideLoginDialog");// 作用是引发事件，以让前台隐藏登陆对话框
        }

        private async void OnNormalLogin()
        {
            await _authService.DoAuthorization();// 因为authService是在打开对话框之前获取的，所以一定不为空且正确

            Lifecycle.AppRuntimeEnvironment.Instance.RemoveUserAccountUnlogin(_account);// 从未登录用户列表中移除
            NotifyPropertyChanged("HideLoginDialog");// 作用是引发事件，以让前台隐藏登陆对话框
        }

        /// <summary>
        /// 用户账户发生更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAccount_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "UserAccount")
            {
                var accounts = Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccount();
                foreach (Models.Account a in accounts)
                {
                    if (a.isAvailable)
                    {
                        switch(a.Sns.Name)
                        {
                            case _weiboSnsName:
                                IsWeiboLogin = true;
                                break;
                            case _kaixinSnsName:
                                IsKaixinLogin = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                IsStartUsingEnable = (IsWeiboLogin || IsKaixinLogin);
            }
        }
    }
}
