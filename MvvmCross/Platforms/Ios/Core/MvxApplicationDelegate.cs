// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Foundation;
using MvvmCross.Core;
using MvvmCross.ViewModels;
using UIKit;

namespace MvvmCross.Platforms.Ios.Core
{
    public abstract class MvxApplicationDelegate : UIApplicationDelegate, IMvxApplicationDelegate
    {
        /// <summary>
        /// UIApplicationDelegate.Window doesn't really exist / work. It was added by Xamarin.iOS templates 
        /// </summary>
        public new virtual UIWindow Window { get; set; }

        public MvxApplicationDelegate() : base()
        {
            RegisterSetup();
        }

        public override void WillEnterForeground(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
        }

        public override void WillTerminate(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.Closing);
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            if (Window == null)
                Window = new UIWindow(UIScreen.MainScreen.Bounds);

            MvxIosSetupSingleton.EnsureSingletonAvailable(this, Window).EnsureInitialized();

            RunAppStart(launchOptions);

            FireLifetimeChanged(MvxLifetimeEvent.Launching);
            return true;
        }

        protected virtual void RunAppStart(object hint = null)
        {
            if (Mvx.IoCProvider?.TryResolve(out IMvxAppStart startup) == true && !startup.IsStarted)
            {
                startup.Start(GetAppStartHint(hint));
            }
            Window.MakeKeyAndVisible();
        }

        protected virtual object GetAppStartHint(object hint = null)
        {
            return hint;
        }

        protected virtual void RegisterSetup()
        {
        }

        private void FireLifetimeChanged(MvxLifetimeEvent which)
        {
            var handler = LifetimeChanged;
            handler?.Invoke(this, new MvxLifetimeEventArgs(which));
        }

        public event EventHandler<MvxLifetimeEventArgs> LifetimeChanged;
    }

    public abstract class MvxApplicationDelegate<TMvxIosSetup, TApplication> : MvxApplicationDelegate
       where TMvxIosSetup : MvxIosSetup<TApplication>, new()
       where TApplication : class, IMvxApplication, new()
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<TMvxIosSetup>();
        }
    }
}
