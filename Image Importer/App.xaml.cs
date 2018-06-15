using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Image_Importer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (args.Verb == "open")
            {
                rootFrame.Navigate(typeof(Views.SingleImageView), args.Files[0]);
            }

            if (args.Verb == "import")
            {
                var folder = (Windows.Storage.StorageFolder)args.Files[0];
                Common.Models.ImageDevice imageDevice = Common.Models.ImageDevice.FromFolder(folder);
                rootFrame.Navigate(typeof(Views.FilesView), imageDevice);
            }

            Window.Current.Activate();

            base.OnFileActivated(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (args.Kind == ActivationKind.Device)
            {
                // Cast the activated event args as DeviceActivatedEventArgs and show images
                var deviceArgs = args as DeviceActivatedEventArgs;
                if (deviceArgs != null)
                {
                    Common.Models.ImageDevice imageDevice = Common.Models.ImageDevice.FromDeviceId(deviceArgs.DeviceInformationId);
                    rootFrame.Navigate(typeof(Views.FilesView), imageDevice);
                }
            }

            Window.Current.Activate();

            base.OnActivated(args);
        }

        #region Settings Page

        //public void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        //{
        //    //SettingsCommand aboutCommand = new SettingsCommand("mainSettings", "Import Settings", (x) =>
        //    //{
        //    //    SettingsFlyout flyout = new SettingsFlyout();
        //    //    flyout.Content = new Views.SettingsMain();
        //    //    flyout.Title = "Import Settings";
        //    //    flyout.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
        //    //    flyout.Show();
        //    //});
        //    //eventArgs.Request.ApplicationCommands.Add(aboutCommand);

        //    SettingsCommand privacyCommand = new SettingsCommand("privacyCommand", "Privacy Policy", (x) =>
        //    {
        //        Uri location = new Uri("http://massivescale.com/pages/apps/image-importer/image-importer-privacy-policy/");
        //        var ignore = Windows.System.Launcher.LaunchUriAsync(location);
        //    });
        //    eventArgs.Request.ApplicationCommands.Add(privacyCommand);
        //}

        #endregion Settings Page

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            this.UnhandledException += App_UnhandledException;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(Views.DevicesView), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }


            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debugger.Launch();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}