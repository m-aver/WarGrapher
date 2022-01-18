using System;

namespace WarGrapher.ViewModels.ViewFactories
{
    #region REMARK
    /*
     * FactoryAccessibleAttribute по сути является просто ключом к интерфейсу фабрики (ViewFactories.WindowFactory), оформленным через рефлексию
     * это сделано, чтобы предотвратить публичный доступ к элементам интерфейса, который реализует уведомления при закрытии представления (окна) таким образом, что ViewModel ничего об этом представлении не знает
     * публичный доступ крайне нежелателен, т.к. при определенных использованиях может привести к критическим ошибкам
     * пожалуй можно было бы реализовать и через привычный интерфейсный тип, но тогда надо инкапсулировать WindowViewModel и WindowFactory в отдельную сборку
     */
    #endregion

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
