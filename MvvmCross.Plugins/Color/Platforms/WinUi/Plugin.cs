// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using MvvmCross.IoC;
using MvvmCross.UI;

namespace MvvmCross.Plugin.Color.Platforms.WinUi
{
    [MvxPlugin]
    public class Plugin : BasePlugin
    {
        public override void Load(IMvxIoCProvider provider)
        {
            base.Load(provider);
            provider.RegisterSingleton<IMvxNativeColor>(new MvxWindowsColor());
        }
    }
}
