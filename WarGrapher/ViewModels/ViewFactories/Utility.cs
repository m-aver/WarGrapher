using System;

namespace WarGrapher.ViewModels.ViewFactories
{
    /// <summary>
    /// Provides private access for <see cref="WindowFactory"/> to the members of a target type through the reflection.
    /// </summary>
    /// <remarks>
    /// The possible member keys are defined in <see cref="WindowFactory"/> 
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    internal sealed class FactoryAccessibleAttribute : Attribute
    {
        public string MemberKey { get; }

        public FactoryAccessibleAttribute(string memberKey)
        {
            MemberKey = memberKey;
        }
    }
}
