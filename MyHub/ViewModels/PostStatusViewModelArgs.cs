using System;

namespace MyHub.ViewModels
{
    /// <summary>
    /// 定义导航到PostStatusPage页面的参数
    /// </summary>
    public class PostStatusViewModelArgs
    {
        public Lifecycle.MyHubEnums.NavigatedToPostStatusPageType NavigationType { get; set; }

        public object Parameter { get; set; }
    }
}
