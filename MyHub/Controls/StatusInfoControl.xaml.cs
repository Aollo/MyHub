using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyHub.Models;
using Windows.UI.Xaml.Documents;
using MyHub.Services;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyHub.Controls
{
    public sealed partial class StatusInfoControl : UserControl
    {
        private string _url = "";// 存放超链接的url

        public StatusInfoControl()
        {
            this.InitializeComponent();
        }

        private void statusContentTextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null)
                return;

            var status = DataContext as Status;
            statusContentTextBlock.Inlines.Clear();
            TranslateContentLink(status.Content, statusContentTextBlock);
        }

        private void retweetedStatusTextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null)
                return;

            var retweetedStatus = (DataContext as Status).RetweetedStatus;
            if (retweetedStatus == null)
                return;

            retweetedStatusTextBlock.Inlines.Clear();
            if(retweetedStatus.Author != null)
            {
                Hyperlink l = new Hyperlink();
                l.UnderlineStyle = UnderlineStyle.None;
                l.Inlines.Add(new Run() { Text = "@" + retweetedStatus.Author.NickName });
                l.Click += AtUserLink_Click;
                retweetedStatusTextBlock.Inlines.Add(l);
                Run r = new Run();
                r.Text = ":";
                retweetedStatusTextBlock.Inlines.Add(r);
            }
            TranslateContentLink(retweetedStatus.Content, retweetedStatusTextBlock);
        }

        /// <summary>
        /// 将新鲜事中的内容Content中的@用户、#标签#、http://...链接解析出来，使用之前需要将原TextBlock的Inlines清空
        /// </summary>
        /// <param name="content"></param>
        private void TranslateContentLink(string content, TextBlock statusContentTextBlock)
        {
            var statusContent = content.ToCharArray();
            int length = statusContent.Length, lastEnd = 0, searchStart = 0, searchEnd = 0, i = 0;
            string tempStr = "";
            for (; searchStart < length; )
            {
                if (statusContent[searchStart] == '@')// 识别@用户的标签
                {
                    for (searchEnd = searchStart + 1; 
                        searchEnd < length 
                        && !char.IsWhiteSpace(statusContent[searchEnd]) 
                        && statusContent[searchEnd] != '@' 
                        && statusContent[searchEnd] != ':'
                        && statusContent[searchEnd] != '，'; 
                        ++searchEnd) ;
                    for (i = lastEnd, tempStr = ""; i <= searchStart - 1; ++i) tempStr += statusContent[i];
                    if (!string.IsNullOrWhiteSpace(tempStr))
                        statusContentTextBlock.Inlines.Add(new Run() { Text = tempStr });
                    for (i = searchStart, tempStr = ""; i <= searchEnd - 1; ++i) tempStr += statusContent[i];
                    if (!string.IsNullOrWhiteSpace(tempStr))
                    {
                        Hyperlink l = new Hyperlink();
                        l.UnderlineStyle = UnderlineStyle.None;
                        l.Inlines.Add(new Run() { Text = tempStr });
                        l.Click += AtUserLink_Click;
                        statusContentTextBlock.Inlines.Add(l);
                    }
                    lastEnd = searchStart = searchEnd;
                }
                else if (statusContent[searchStart] == '#')// 识别话题的标签
                {
                    for (searchEnd = searchStart + 1; searchEnd < length && statusContent[searchEnd] != '#'; ++searchEnd) ;
                    if (searchEnd >= length)
                    {
                        for (i = lastEnd, tempStr = ""; i < length; ++i) tempStr += statusContent[i];
                        if (!string.IsNullOrWhiteSpace(tempStr))
                            statusContentTextBlock.Inlines.Add(new Run() { Text = tempStr });
                        break;
                    }
                    for (i = lastEnd, tempStr = ""; i <= searchStart - 1; ++i) tempStr += statusContent[i];
                    if (!string.IsNullOrWhiteSpace(tempStr))
                        statusContentTextBlock.Inlines.Add(new Run() { Text = tempStr });
                    for (i = searchStart, tempStr = ""; i <= searchEnd; ++i) tempStr += statusContent[i];//#..#
                    if (!string.IsNullOrWhiteSpace(tempStr))
                    {
                        Hyperlink l = new Hyperlink();
                        l.UnderlineStyle = UnderlineStyle.None;
                        l.Inlines.Add(new Run() { Text = tempStr });
                        statusContentTextBlock.Inlines.Add(l);
                    }
                    lastEnd = searchStart = searchEnd + 1;
                }
                else if (statusContent.Length > 7
                    && statusContent[searchStart] == 'h'
                    && statusContent[searchStart + 1] == 't'
                    && statusContent[searchStart + 2] == 't'
                    && statusContent[searchStart + 3] == 'p'
                    && statusContent[searchStart + 4] == ':'
                    && statusContent[searchStart + 5] == '/'
                    && statusContent[searchStart + 6] == '/')// 识别超链接的标签
                {
                    for (searchEnd = searchStart + 7;
                        searchEnd < length
                        && (statusContent[searchEnd] >= 33 && statusContent[searchEnd] <= 126);
                        ++searchEnd) ;

                    for (i = lastEnd, tempStr = ""; i <= searchStart - 1; ++i) tempStr += statusContent[i];
                    if (!string.IsNullOrWhiteSpace(tempStr))
                        statusContentTextBlock.Inlines.Add(new Run() { Text = tempStr });
                    for (i = searchStart, tempStr = ""; i <= searchEnd - 1; ++i) tempStr += statusContent[i];// http://....
                    if (!string.IsNullOrWhiteSpace(tempStr))
                    {
                        _url = tempStr;

                        Hyperlink l = new Hyperlink();
                        l.UnderlineStyle = UnderlineStyle.None;
                        l.Inlines.Add(new Run() { Text = "网页链接" });
                        //l.NavigateUri = new Uri(tempStr);
                        l.Click += Weblink_Click;
                        statusContentTextBlock.Inlines.Add(l);
                    }
                    else
                        _url = "";
                    lastEnd = searchStart = searchEnd;
                }
                else
                {
                    ++searchStart;
                }
            }

            for (i = lastEnd, tempStr = ""; i < length; ++i) tempStr += statusContent[i];
            statusContentTextBlock.Inlines.Add(new Run() { Text = tempStr });
        }

        private void Weblink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Facade.NavigationFacade.NavigateToWebViewerPage(new Uri(_url));
        }

        /// <summary>
        /// 当@用户的链接被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void AtUserLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<ISnsDataService>((DataContext as Status)?.Sns.Name);
            if (service == null)
                return;

            var userName = (sender.Inlines.ElementAt(0) as Run)?.Text?.Substring(1);
            var userProfile = await service.GetUserProfile("", userName);// 根据用户名提取用户资料
            if (userProfile == null)
                return;

            Facade.NavigationFacade.NavigateToAccountDetailPage(userProfile);
        }
    }
}
