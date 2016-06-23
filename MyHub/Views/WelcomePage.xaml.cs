using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyHub.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WelcomePage : BasePage
    {
        private WelcomeViewModel _viewModel;

        public WelcomePage()
        {
            this.InitializeComponent();

            _viewModel = new WelcomeViewModel();
            DataContext = _viewModel;
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
        }

        private async void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ShowLoginDialog")// 打开登陆对话框
            {
                await loginContentDialog.ShowAsync();
            }
            else if(e.PropertyName == "HideLoginDialog")
            {
                loginContentDialog.Hide();
            }
        }
    }
}
