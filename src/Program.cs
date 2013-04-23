using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using AutoBrowserChooser.Properties;
using Microsoft.Win32;

namespace AutoBrowserChooser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                CheckRegistry();
            }
            catch (UnauthorizedAccessException) { }

            if (args.Length < 1)
            {
                new ConfigWindow().ShowDialog();
                return;
            }

            string url = args[0];
            bool isIntranetUrl = false;
            bool isLocalhost = false;
            bool isClickOnceUrl = false;
            string authority = string.Empty;

            try
            {
                Uri uri = new Uri(url);
                authority = uri.Authority.ToLower();
                isIntranetUrl = !authority.Contains('.');
                isLocalhost = uri.IsLoopback;
                isClickOnceUrl = uri.AbsolutePath.EndsWith(".application", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (UriFormatException)
            {
                // Nothing we can do about this, attempt to pass it on
            }

            if ((Settings.Default.LaunchAlternateForLocalhost && isLocalhost) ||
                (Settings.Default.LaunchAlternateForIntranet && isIntranetUrl && !isLocalhost) ||
                (Settings.Default.LaunchAlternateForDomains.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(
                    domain => authority.Contains(domain.ToLower()))) ||
                (Settings.Default.LaunchAlternateForClickOnce && isClickOnceUrl))
            {
                LaunchBrowser(Settings.Default.AlternateBrowser, url);
            }
            else
            {
                LaunchBrowser(Settings.Default.MainBrowser, url);
            }
        }

        private static void LaunchBrowser(string browser, string url)
        {
            browser = browser.ToLower();
            foreach (var name in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                browser = browser.Replace("{" + name.ToLower() + "}", Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), name)));
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = browser;
            psi.Arguments = url;
            psi.WorkingDirectory = Path.GetDirectoryName(browser);
            Process.Start(psi);
        }

        static void CheckRegistry()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            Registry.ClassesRoot.CreateSubKey(@"AutoBrowserChooserHTML\DefaultIcon").SetValue("", path + ",0", RegistryValueKind.String);
            Registry.ClassesRoot.CreateSubKey(@"AutoBrowserChooserHTML\shell\open\command").SetValue("", path + " %1", RegistryValueKind.String);
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities").SetValue("ApplicationIcon", path + ",0", RegistryValueKind.String);

            string currentValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice", "Progid", null) as string;

            if (currentValue != "AutoBrowserChooserHTML")
            {
                if (MessageBox.Show("Set AutoBrowserChooser as default browser?", "AutoBrowserChooser", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Registry.ClassesRoot.CreateSubKey(@"AutoBrowserChooserHTML").SetValue("", "AutoBrowserChooser HTML", RegistryValueKind.String);
                    Registry.ClassesRoot.CreateSubKey(@"AutoBrowserChooserHTML").SetValue("URL Protocol", "", RegistryValueKind.String);
                    Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\ftp\UserChoice").SetValue("Progid", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice").SetValue("Progid", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice").SetValue("Progid", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities").SetValue("ApplicationDescription", "Simple app that opens intranet links in one browser, and normal links in another.", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities").SetValue("ApplicationName", "AutoBrowserChooser", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\FileAssociations").SetValue(".htm", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\FileAssociations").SetValue(".html", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\FileAssociations").SetValue(".shtml", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\FileAssociations").SetValue(".xht", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\FileAssociations").SetValue(".xhtml", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\StartMenu").SetValue("StartMenuInternet", "AutoBrowserChooser.exe", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\URLAssociations").SetValue("ftp", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\URLAssociations").SetValue("http", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\AutoBrowserChooser\Capabilities\URLAssociations").SetValue("https", "AutoBrowserChooserHTML", RegistryValueKind.String);
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\RegisteredApplications").SetValue("AutoBrowserChooser", "Software\\AutoBrowserChooser\\Capabilities", RegistryValueKind.String);
                }
            }
        }
    }
}
