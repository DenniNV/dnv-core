using System;
using Libraries.DNV.CodePresets;

namespace Libraries.DNV.DependencyInjection.Context
{
    public interface IBaseBuilder : IAsyncBuilder, IDisposable {}
}