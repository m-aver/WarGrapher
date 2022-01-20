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
