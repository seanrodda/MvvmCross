// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using MvvmCross.Base;
using MvvmCross.Binding;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Binders;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Binding.Combiners;
using MvvmCross.Converters;
using MvvmCross.IoC;
using MvvmCross.Platforms.Wpf.Binding.MvxBinding;
using MvvmCross.Platforms.Wpf.Binding.MvxBinding.Target;
using MvvmCross.Platforms.Wpf.Binding.WindowsBinding;

namespace MvvmCross.Platforms.Wpf.Binding
{
    public class MvxWindowsBindingBuilder : MvxBindingBuilder
    {
        public enum BindingType
        {
            Windows,
            MvvmCross
        }

        private readonly BindingType _bindingType;
        private readonly Action<IMvxTargetBindingFactoryRegistry> _fillTargetFactories;
        private readonly Action<IMvxBindingNameRegistry> _fillBindingNames;
        private readonly Action<IMvxValueConverterRegistry> _fillValueConverters;
        private readonly Action<IMvxValueCombinerRegistry> _fillValueCombiners;

        public MvxWindowsBindingBuilder(
            Action<IMvxTargetBindingFactoryRegistry> fillTargetFactories = null,
            Action<IMvxBindingNameRegistry> fillBindingNames = null,
            Action<IMvxValueConverterRegistry> fillValueConverters = null,
            Action<IMvxValueCombinerRegistry> fillValueCombiners = null,
            BindingType bindingType = BindingType.MvvmCross)
        {
            _fillTargetFactories = fillTargetFactories;
            _fillBindingNames = fillBindingNames;
            _fillValueConverters = fillValueConverters;
            _fillValueCombiners = fillValueCombiners;
            _bindingType = bindingType;
        }

        public override void DoRegistration(IMvxIoCProvider iocProvider)
        {
            base.DoRegistration(iocProvider);
            InitializeBindingCreator();
        }

        protected override void RegisterBindingFactories(IMvxIoCProvider iocProvider)
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    // no need for MvvmCross binding factories - so don't create them
                    break;

                case BindingType.MvvmCross:
                    base.RegisterBindingFactories(iocProvider);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override IMvxTargetBindingFactoryRegistry CreateTargetBindingRegistry()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return base.CreateTargetBindingRegistry();

                case BindingType.MvvmCross:
                    return new MvxWindowsTargetBindingFactoryRegistry();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitializeBindingCreator()
        {
            var creator = CreateBindingCreator();
            Mvx.IoCProvider.RegisterSingleton(creator);
        }

        protected virtual IMvxBindingCreator CreateBindingCreator()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return new MvxWindowsBindingCreator();

                case BindingType.MvvmCross:
                    return new MvxMvvmCrossBindingCreator();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);

            if (MvxSingleton<IMvxWindowsAssemblyCache>.Instance != null)
            {
                foreach (var assembly in MvxSingleton<IMvxWindowsAssemblyCache>.Instance.Assemblies)
                {
                    registry.Fill(assembly);
                }
            }

            _fillValueConverters?.Invoke(registry);
        }

        protected override void FillValueCombiners(IMvxValueCombinerRegistry registry)
        {
            base.FillValueCombiners(registry);

            if (MvxSingleton<IMvxWindowsAssemblyCache>.Instance != null)
            {
                foreach (var assembly in MvxSingleton<IMvxWindowsAssemblyCache>.Instance.Assemblies)
                {
                    registry.Fill(assembly);
                }
            }

            _fillValueCombiners?.Invoke(registry);
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Visible,
                view => new MvxVisibleTargetBinding(view));

            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Collapsed,
                view => new MvxCollapsedTargetBinding(view));

            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Hidden,
                view => new MvxCollapsedTargetBinding(view));

            base.FillTargetFactories(registry);

            _fillTargetFactories?.Invoke(registry);
        }

        protected override void FillDefaultBindingNames(IMvxBindingNameRegistry registry)
        {
            base.FillDefaultBindingNames(registry);
            _fillBindingNames?.Invoke(registry);
        }
    }
}
