using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using MyHub.ComponentModel;
using MyHub.Models;

namespace MyHub.Lifecycle
{
    /// <summary>
    /// 应用程序运行时环境，保存所有应用程序运行过程中需要用到的全局数据
    /// 是应用的全局数据中心
    /// </summary>
    public class AppRuntimeEnvironment : ObservableObjectBase
    {
        // TODO: 完善各个社交网络支持的功能，注意与资源字典中的支持字段相对应
        private static readonly List<string> _weiboFatureSupported = new List<string>()
        {
            "get_status", "post_status"
        };
        private static readonly List<string> _kaixinFatureSupported = new List<string>()
        {
            "get_status", "post_status"
        };

        private Dictionary<string, Account> _snsUserAccountDict;

        private Dictionary<string, Account> _snsUserAccountUnloginDict;// 保存没有登录的、但是没有过期的用户账户信息

        private Dictionary<MyHubEnums.NavigationFrameType, Frame> _frameDict;

        /// <summary>
        /// 私有构造函数，禁止实例化
        /// </summary>
        private AppRuntimeEnvironment()
        {
            _snsUserAccountDict = new Dictionary<string, Account>();
            _snsUserAccountUnloginDict = new Dictionary<string, Account>();
            _frameDict = new Dictionary<MyHubEnums.NavigationFrameType, Frame>();
        }

        /// <summary>
        /// 类的单例
        /// </summary>
        public static readonly AppRuntimeEnvironment Instance = new AppRuntimeEnvironment();

        /// <summary>
        /// 判断某一社交网络是否支持某个功能
        /// </summary>
        /// <param name="snsName">社交网络的中文全称，如新浪微博、开心网</param>
        /// <param name="fature">社交功能的英文</param>
        /// <returns></returns>
        public static bool IsSnsFatureSupport(string snsName, string fature)
        {
            if (string.IsNullOrWhiteSpace(fature))
                return false;

            bool result;

            switch(snsName)
            {
                case "新浪微博":
                    result = _weiboFatureSupported.Contains(fature);
                    break;
                case "开心网":
                    result = _kaixinFatureSupported.Contains(fature);
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }
        
        /// <summary>
        /// 当用户账户发生更改的时候引发事件
        /// </summary>
        public EventHandler<Account> UserAccountChanged;

        /// <summary>
        /// 获取所有用户账户，即_snsUserAccountDict和_snsUserAccountUnloginDict中的所有账户
        /// </summary>
        /// <returns></returns>
        public Account[] GetAllUserAccountWithUnlogin()
        {
            Account[] accounts = new Account[_snsUserAccountDict.Count + _snsUserAccountUnloginDict.Count];
            _snsUserAccountDict.Values.CopyTo(accounts, 0);
            _snsUserAccountUnloginDict.Values.CopyTo(accounts, _snsUserAccountDict.Count);
            return accounts;
        }

        /// <summary>
        /// 获取登录的用户账户信息
        /// 当前只支持每个社交网络登录一个账号，如果需要支持多账号需要修改此处
        /// </summary>
        /// <param name="snsName">社交账号类型名字，需要与Account.SnsType.SnsName相对应</param>
        /// <returns></returns>
        public Account GetUserAccount(string snsName)
        {
            if (_snsUserAccountDict.ContainsKey(snsName))
                return _snsUserAccountDict[snsName];

            return null;
        }

        public Account[] GetAllUserAccount()
        {
            Account[] accounts = new Account[_snsUserAccountDict.Count];
            _snsUserAccountDict.Values.CopyTo(accounts, 0);
            return accounts;
        }

        /// <summary>
        /// 设置登录的用户账户信息，通过社交网络的名字进行索引
        /// 当前只支持每个社交网络登录一个账号，如果需要支持多账号需要修改此处
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool SetUserAccount(Account account)
        {
            bool needNotify = false;

            if (_snsUserAccountDict.ContainsKey(account.Sns.Name))// 如果包含key，则修改账户信息
            {
                if(_snsUserAccountDict[account.Sns.Name] != account)
                {
                    _snsUserAccountDict[account.Sns.Name] = account;
                    needNotify = true;
                }
            }
            else// 如果没有此账户信息，添加
            {
                _snsUserAccountDict.Add(account.Sns.Name, account);
                needNotify = true;
            }

            if(needNotify)
            {
                NotifyPropertyChanged("UserAccount");
                UserAccountChanged?.Invoke(this, account);
            }
            
            return needNotify;
        }

        /// <summary>
        /// 将当前登录用户列表中的一个账户移除，为了满足快速登陆、退出功能的需要
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool RemoveUserAccount(Account account)
        {
            bool needNotify = false;

            if (_snsUserAccountDict.ContainsKey(account.Sns.Name))
            {
                _snsUserAccountDict.Remove(account.Sns.Name);
                needNotify = true;
            }

            //if (needNotify)
            //{
            //    NotifyPropertyChanged("UserAccount");
            //    UserAccountChanged?.Invoke(this, account);
            //}

            return needNotify;
        }

        /// <summary>
        /// 获取没有登录，但是授权没有过期的用户账户
        /// </summary>
        /// <param name="snsName"></param>
        /// <returns></returns>
        public Account GetUserAccountUnlogin(string snsName)
        {
            if (_snsUserAccountUnloginDict.ContainsKey(snsName))
                return _snsUserAccountUnloginDict[snsName];

            return null;
        }

        /// <summary>
        /// 获取所有没有登录但是授权没有过期的用户账户
        /// </summary>
        /// <returns></returns>
        public Account[] GetAllUserAccountUnlogin()
        {
            Account[] accounts = new Account[_snsUserAccountUnloginDict.Count];
            _snsUserAccountUnloginDict.Values.CopyTo(accounts, 0);
            return accounts;
        }

        public bool SetUserAccountUnlogin(Account account)
        {
            bool needNotify = false;

            if (_snsUserAccountUnloginDict.ContainsKey(account.Sns.Name))// 如果包含key，则修改账户信息
            {
                if(_snsUserAccountUnloginDict[account.Sns.Name] != account)
                {
                    _snsUserAccountUnloginDict[account.Sns.Name] = account;
                    needNotify = true;
                }
            }
            else// 如果没有此账户信息，添加。此路径正常情况是不可能发生的
            {
                _snsUserAccountUnloginDict.Add(account.Sns.Name, account);
                needNotify = true;
            }

            if (needNotify)
            {
                NotifyPropertyChanged("UserAccountUnlogin");
                UserAccountChanged?.Invoke(this, account);
            }

            return needNotify;
        }

        public bool RemoveUserAccountUnlogin(Account account)
        {
            bool needNotify = false;

            if (_snsUserAccountUnloginDict.ContainsKey(account.Sns.Name))
            {
                _snsUserAccountUnloginDict.Remove(account.Sns.Name);
                needNotify = true;
            }

            //if (needNotify)
            //{
            //    NotifyPropertyChanged("UserAccountUnlogin");
            //    UserAccountChanged?.Invoke(this, account);
            //}

            return needNotify;
        }

        public void SetFrame(MyHubEnums.NavigationFrameType type, Frame frame)
        {
            if (frame == null)
                return;

            if (_frameDict.ContainsKey(type))
                _frameDict[type] = frame;
            else
                _frameDict.Add(type, frame);
        }

        public Frame GetFrame(MyHubEnums.NavigationFrameType type)
        {
            if (_frameDict.ContainsKey(type))
                return _frameDict[type];

            return null;
        }
    }
}
