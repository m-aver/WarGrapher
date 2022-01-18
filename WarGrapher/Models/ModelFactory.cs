#region REMARK
/*
 * насколько я понимаю, нет смысла добавлять потокобезопасноть в этот код,
 * т.к. статический конструктор по умолчанию потокобезопасен
 * но могу ошибаться, т.к. лишь поверхностно глянул источники
 * 
 * SOURCES:
 * https://metanit.com/sharp/patterns/2.3.php
 * https://habr.com/ru/post/125421/
 */
#endregion

namespace WarGrapher.Models
{
    /// <summary>
    /// Serves for creating and storing the model instance that is common for each application module.
    /// </summary>
    /// <remarks>
    /// If necessary, you can easily change the specific implementation of the model.
    /// </remarks>
    static class ModelFactory
    {
        /// <summary>
        /// Gets the shared model instance.
        /// </summary>
        public static IModel ModelInstance { get; }

        static ModelFactory()
        {
            ModelInstance = new Model();
        }
    }
}
