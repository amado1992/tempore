namespace Tempore.Tests.Infraestructure;

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

using global::Tempore.Tests.Infraestructure.Extensions;

using MethodDecorator.Fody.Interfaces;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class SetCultureAttribute : Attribute, IMethodDecorator
{
    private object? instance;
    private MethodBase? method;
    private object[]? parameters;

    private CultureInfo? originalCulture;
    private CultureInfo? originalUserInterfaceCulture;

    public void Init(object instance, MethodBase method, object[] parameters)
    {
        this.instance = instance;
        this.method = method;
        this.parameters = parameters;
    }

    public void OnEntry()
    {
        int idx = Array.FindIndex(this.method!.GetParameters(), info => info.ParameterType == typeof(CultureInfo));
        if (idx >= 0 && this.parameters![idx] is CultureInfo cultureInfo)
        {
            this.originalCulture = Thread.CurrentThread.CurrentCulture;
            this.originalUserInterfaceCulture = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.SetCulture(cultureInfo);
        }
    }

    public void OnExit()
    {
        this.ResetCulture();
    }

    public void OnException(Exception exception)
    {
        this.ResetCulture();
    }

    private void ResetCulture()
    {
        if (this.originalCulture is not null)
        {
            Thread.CurrentThread.SetCulture(this.originalCulture, this.originalUserInterfaceCulture);
        }
    }
}