using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MyHub.Models;
using MyHub.Services;
using MyHub.Commands;

namespace MyHub.ViewModels
{
    public class AccountManagementViewModel : ViewModelBase
    {
        private ObservableCollection<Tuple<Account, UserProfile>> _userAccountProfileList;

        public AccountManagementViewModel()
        {
            _userAccountProfileList = new ObservableCollection<Tuple<Account, UserProfile>>();

            LogoutCommand = new RelayCommand<Account>(OnLogoutCommandAct);
            QuickLoginCommand = new RelayCommand<Account>(OnQuickLoginCommandAct);
            ReloginCommand = new RelayCommand<Account>(OnReloginCommandAct);
            Lifecycle.AppRuntimeEnvironment.Instance.UserAccountChanged += UserAccount_PropertyChanged;
        }

        public RelayCommand<Account> LogoutCommand { get; set; }

        public RelayCommand<Account> QuickLoginCommand { get; set; }

        public RelayCommand<Account> ReloginCommand { get; set; }

        public ObservableCollection<Tuple<Account, UserProfile>> UserAccountProfileList
        {
            get { return _userAccountProfileList; }
            private set
            {
                if (_userAccountProfileList != value)
                {
                    _userAccountProfileList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            ISnsDataService service;
            UserProfile tempUserProfile;
            Account[] accounts = Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccountWithUnlogin();
            if (accounts == null && accounts.Length <= 0) return;

            UserAccountProfileList.Clear();
            foreach(Account account in accounts)
            {
                service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(account.Sns.Name);
                tempUserProfile = await service.GetUserProfile(account.UserId, "");
                UserAccountProfileList.Add(new Tuple<Account, UserProfile>(account, tempUserProfile));
            }
        }

        private void OnLogoutCommandAct(Account account)
        {
            // 账户退出要做的三件事情：
            // 1.修改UserAccountProfileList对应的账户项的isAvailable，并引发通知以更新账户显示
            // 2.修改全局数据中心的登录账户列表与未登录账户列表
            // 3.将更新之后的账户存入数据库
            // 做这三件事的方式是：更新账户isAvailable、修改全局数据中心的登录账户列表并在引发通知的事件处理中做剩余的事

            // 更新对应账户的isAvailable，表示账户退出
            account.isAvailable = false;
            // 通过set方法引发属性更改事件，在事件处理里根据account.isAvailable调用RemoveUserAccountUnlogin方法
            Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccountUnlogin(account);
        }

        private void OnQuickLoginCommandAct(Account account)
        {
            account.isAvailable = true;
            Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccount(account);
        }

        private async void OnReloginCommandAct(Account account)
        {
            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IAuthorizationService>(account.Sns.Name);
            // 执行重新授权，在授权方法内部会调用Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccount(account);引发通知
            await service.DoAuthorization();
        }

        /// <summary>
        /// 用户账户更改时引发的事件的处理；
        /// 做的事情包括：根据account.isAvailable调用对应的Remove方法、修改UserAccountProfileList并通知界面更新、保存到数据库；
        /// 要保证逻辑正确，只能是在产生事件之前更新正确的account.isAvailable属性，并且只能通过Set方法引发事件
        /// </summary>
        private async void UserAccount_PropertyChanged(object sender, Account account)
        {
            if(account.isAvailable)// 更新之后是可用，则之前不可用
            {
                //Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccount(account);
                Lifecycle.AppRuntimeEnvironment.Instance.RemoveUserAccountUnlogin(account);
            }
            else// 更新之后不可用
            {
                //Lifecycle.AppRuntimeEnvironment.Instance.SetUserAccountUnlogin(account);
                Lifecycle.AppRuntimeEnvironment.Instance.RemoveUserAccount(account);
            }

            // 把更新后的账户存入数据库
            var dbService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILocalDataService>();
            dbService.StoreAccount(account);

            // 如果退出账号，所有账号都被退出，导航到登录界面
            if (Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccount().Length == 0)
            {
                Facade.NavigationFacade.NavigateToWelcomePage();
                return;
            }

            // 重新载入状态、更新页面显示
            await LoadState();
        }
    }
}
