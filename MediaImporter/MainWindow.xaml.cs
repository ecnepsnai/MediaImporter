namespace MediaImporter
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;
    using System.Linq;
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using WinRT.Interop;

    public sealed partial class MainWindow : Window
    {
        public static AppWindow m_AppWindow { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();

            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.Title = "Media Importer";
            m_AppWindow.SetIcon(@"Assets\icon.ico");
            m_AppWindow.MoveAndResize(new Windows.Graphics.RectInt32 { X = m_AppWindow.Position.X, Y = m_AppWindow.Position.Y, Height = 700, Width = 465 });

            navigationView.SelectedItem = navigationView.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(
                       typeof(Views.ImportPage),
                       null,
                       new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo()
                       );
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void navigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItem selected = (NavigationViewItem)sender.SelectedItem;
            switch (selected.Tag)
            {
                case "Import":
                    ContentFrame.Navigate(typeof(Views.ImportPage), null, args.RecommendedNavigationTransitionInfo);
                    break;
                case "Settings":
                    ContentFrame.Navigate(typeof(Views.SettingsPage), null, args.RecommendedNavigationTransitionInfo);
                    break;
            }
        }
    }
}
