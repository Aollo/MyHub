using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MyHub.ViewModels;
using MyHub.Models;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostStatusPage : BasePage
    {
        private PostStatusViewModel _viewModel;

        public PostStatusPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            _viewModel = null;
            Lifecycle.AppRuntimeEnvironment.Instance.PropertyChanged += UserAccount_PropertyChanged;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(_viewModel == null)// 如果是第一次初始化页面
            {
                _viewModel = new PostStatusViewModel();
                DataContext = _viewModel;
                await UpdateViewState();// 更新界面显示
            }

            // 处理收到的参数，根据参数执行初始化
            if(e.Parameter is PostStatusViewModelArgs)
            {
                var parameter = e.Parameter as PostStatusViewModelArgs;
                switch(parameter.NavigationType)
                {
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.PostNewStatus:
                        statusTextBlock.Text = "发布新鲜事";
                        DoInit();
                        insertLoactionButton.Visibility = Visibility.Visible;
                        pictureAppBarButton.Visibility = Visibility.Visible;
                        snsChecks.Visibility = Visibility.Visible;

                        _viewModel.NavigatedType = parameter.NavigationType;
                        _viewModel.SourcePage = typeof(MyHubBlankPage);// 发布成功之后导航到空白页
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Repost:
                        DoInit();
                        statusTextBlock.Text = "转发新鲜事";
                        // 进行一些初始化，禁用一些功能
                        insertLoactionButton.Visibility = Visibility.Collapsed;
                        pictureAppBarButton.Visibility = Visibility.Collapsed;
                        snsChecks.Visibility = Visibility.Visible;

                        _viewModel.NavigatedType = parameter.NavigationType;
                        _viewModel.SourcePage = typeof(MyHubBlankPage);// 发布成功之后导航到空白页
                        if (parameter.Parameter is Status)
                        {
                            _viewModel.SourceStatus = parameter.Parameter as Status;
                        }
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Comment:
                        DoInit();
                        statusTextBlock.Text = "发布评论";
                        insertLoactionButton.Visibility = Visibility.Collapsed;
                        pictureAppBarButton.Visibility = Visibility.Collapsed;
                        snsChecks.Visibility = Visibility.Collapsed;// 不允许选择发布到的社交网络类型

                        _viewModel.NavigatedType = parameter.NavigationType;
                        _viewModel.SourcePage = typeof(MyHubBlankPage);// 发布成功之后导航到空白页
                        if (parameter.Parameter is Status)
                        {
                            _viewModel.SourceStatus = parameter.Parameter as Status;
                        }
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.ReplyComment:
                        DoInit();
                        statusTextBlock.Text = "回复评论";
                        insertLoactionButton.Visibility = Visibility.Collapsed;
                        pictureAppBarButton.Visibility = Visibility.Collapsed;
                        snsChecks.Visibility = Visibility.Collapsed;// 不允许选择发布到的社交网络类型

                        _viewModel.NavigatedType = parameter.NavigationType;
                        _viewModel.SourcePage = typeof(MyHubBlankPage);// 发布成功之后导航到空白页
                        if (parameter.Parameter is Comment)
                        {
                            _viewModel.SourceComment = parameter.Parameter as Comment;
                        }
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.RepostComment:
                        DoInit();
                        statusTextBlock.Text = "转发评论";
                        insertLoactionButton.Visibility = Visibility.Collapsed;
                        pictureAppBarButton.Visibility = Visibility.Collapsed;
                        snsChecks.Visibility = Visibility.Visible;

                        _viewModel.NavigatedType = parameter.NavigationType;
                        _viewModel.SourcePage = typeof(MyHubBlankPage);// 发布成功之后导航到空白页
                        if (parameter.Parameter is Comment)
                        {
                            var comment = parameter.Parameter as Comment;
                            _viewModel.SourceComment = comment;
                            publishTextBox.Text += "//@" + comment.Author.NickName + ":" + comment.Content;
                        }
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.TransferHotTopic:
                        if (parameter.Parameter is string)
                            publishTextBox.Text += parameter.Parameter as string;
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.TransferMentionUser:
                        if(parameter.Parameter is User)
                        {
                            var user = parameter.Parameter as User;
                            publishTextBox.Text += string.Format("@{0} ", user.NickName);

                            // 清空发布到的社交网络的除此用户之外的选择框
                            var snsTypeCheckedItems = from u in snsChecks.Children
                                                      where (u is CheckBox) && (u as CheckBox).IsEnabled == true 
                                                                            && (u as CheckBox).IsChecked == true
                                                      select u as CheckBox;
                            foreach(CheckBox c in snsTypeCheckedItems)
                            {
                                if ((c.Content as string) == user.Sns.Name)
                                    c.IsChecked = true;
                                else
                                    c.IsChecked = false;
                            }
                        }
                        break;
                    case Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.TransferLocation:
                        if(parameter.Parameter is Location)
                        {
                            // 将Locaiton设置到_viewModel的属性
                            _viewModel.Location = parameter.Parameter as Location;
                            insertedLocationTextBlock.Text = _viewModel.Location?.Title;
                            insertedLocationTextBlock.Visibility = Visibility.Visible;
                        }
                        break;
                    default:
                        break;
                }
            }

            // 执行更多的初始化
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }

        /// <summary>
        /// 当用户账户发生登录退出时，重新载入界面状态
        /// </summary>
        private async void UserAccount_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await UpdateViewState();
        }

        /// <summary>
        /// 更新界面显示状态，包括重新载入状态、更新社交网络选择框的状态
        /// </summary>
        private async Task UpdateViewState()
        {
            await _viewModel.LoadState();// 每一次导航到此页面都会重新LoadState()是为了满足账户登陆退出的功能

            snsChecks.Children.Clear();// 更新界面加入复选框，默认全部选中，下面根据收到的导航参数进一步调整
            foreach (string s in _viewModel.SnsTypeList)
            {
                CheckBox c = new CheckBox();
                c.Content = s;
                c.Checked += OnCheckboxChecked;
                c.Unchecked += OnCheckboxUnchecked;
                c.IsChecked = true;
                snsChecks.Children.Add(c);
            }

            DoInit();
        }

        private void DoInit()
        {
            publishTextBox.Text = "";
        }

        #region 界面相关事件

        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            var snsType = (sender as CheckBox).Content as string;
            _viewModel.CheckedSnsTypeList.Add(snsType);
        }

        private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
        {
            var snsType = (sender as CheckBox).Content as string;
            _viewModel.CheckedSnsTypeList.Remove(snsType);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is string)
                publishTextBox.Text += e.ClickedItem as string;
            //((sender as GridView).SelectedItem as GridViewItem).IsSelected = false;
        }

        private void emotionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            emotionGrid.Visibility = emotionGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        // 当点击插入位置之后显示的插入位置的文字之后，删除插入的位置
        private void insertedLocationTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            insertedLocationTextBlock.Visibility = Visibility.Collapsed;
            _viewModel.Location = null;
        }

        #endregion 界面相关事件

        //private async System.Threading.Tasks.Task<StorageFile> CopyToIso(StorageFile sourceFile)
        //{
        //    var dataBuffer = await FileIO.ReadBufferAsync(sourceFile);
        //    var folder = ApplicationData.Current.LocalFolder;
        //    var destFile = await folder.CreateFileAsync(sourceFile.Name, CreationCollisionOption.ReplaceExisting);
        //    await FileIO.WriteBufferAsync(destFile, dataBuffer);
        //    return destFile;
        //}
    }
}
