using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace WarGrapher.Common
{
    /// <summary>
    /// The class containing extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Retrives all nonabstract classes derived from the this type. 
        /// Searches in this assembly and also in all asemblies that located at the application directory.
        /// </summary>
        internal static IEnumerable<Type> GetAllDerivedClasses(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            //retrieving types from this assembly
            var types = new List<Type>();
            types.AddRange(Assembly.GetExecutingAssembly().GetAllDerivedClasses(type, false));

            //retrieving types from foreign assemblies that is allocated in the root application folder as .dll files
            var dirPath = AppDomain.CurrentDomain.BaseDirectory;
            var dirInfo = new DirectoryInfo(dirPath);

            var asmNames = from file in dirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly)
                           where file.Extension == ".dll"
                           select AssemblyName.GetAssemblyName(file.FullName);

            foreach (var an in asmNames)
            {
                var asm = Assembly.Load(an);
                types.AddRange(asm.GetAllDerivedClasses(type, true));
            }
            return types;
        }

        /// <summary>
        /// Retrives from this assembly all nonabstract classes derived from the passed type. 
        /// </summary>
        /// <param name="type">The type for that perform a search</param>
        /// <param name="onlyPublic">Determines whether only public classes or all classes will be returned</param>
        private static IEnumerable<Type> GetAllDerivedClasses(this Assembly assembly, Type type, bool onlyPublic)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var types = onlyPublic ? assembly.GetExportedTypes() : assembly.GetTypes();
            return types.Where(
                t =>
                t != type &&
                t.IsClass &&
                !t.IsAbstract &&
                type.IsAssignableFrom(t));
        }


        /// <summary>
        /// Executes an action for each item in this enumerable object
        /// </summary>
        /// <typeparam name="T">A type of items in this enumeration</typeparam>
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (T o in e)
            {
                action(o);
            }
        }

        /// <summary>
        /// Executes an action for each item in this enumerable object
        /// </summary>
        public static void ForEach(this IEnumerable e, Action<object> action)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (object o in e)
            {
                action(o);
            }
        }

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified enumeration constant. 
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="constant">The enumeration constant to inspect.</param>
        /// <returns>A custom attribute that matches T, or null if the attribute is not found.</returns>
        internal static T GetEnumConstAttribute<T>(this Enum constant) where T : Attribute
        {
            if (constant == null) throw new ArgumentNullException(nameof(constant));

            var constantName = constant.ToString();
            var attr = constant.GetType().GetField(constantName).GetCustomAttribute<T>(false);
            return attr;
        }

        /// <summary>
        /// Performs a cast of an enumeration constant from <see cref="BodyPart"/> to <see cref="EquipType"/> 
        /// </summary>
        /// <param name="bodyPart">The constant to cast</param>
        /// <returns>Converted value of an equipment type</returns>
        public static EquipType CastToEquipment(this BodyPart bodyPart)
        {
            var attr = bodyPart.GetEnumConstAttribute<BodyPartCastAttribute>();
            if (attr == null)
            {
                throw new InvalidOperationException(
                    "The conversion is not supported because the invoking constant does not have "
                    + nameof(BodyPartCastAttribute));
            }
            return attr.EquipType;
        }

        /// <summary>
        /// Determines whether the window is closed or not.
        /// </summary>
        internal static bool IsClosed(this Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            IEnumerable<Window> appWinCollection = Application.Current.Windows.OfType<Window>();
            return !appWinCollection.Any(win => Object.ReferenceEquals(win, window));
        }

        /// <summary>
        /// Retrieves all bitwise constants from this enumeration value 
        /// </summary>
        internal static Enum[] GetFlags(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.GetType().GetCustomAttribute<FlagsAttribute>() == null)
                throw new ArgumentException("The target enumeration should be noted with the Flags attribute", nameof(value));

            return
                Enum.GetValues(value.GetType())
                .OfType<Enum>()
                .Where(v => value.HasFlag(v))
                .ToArray();
        }

        /// <summary>
        /// Retrieves a multiline string that composes of exception stack messages.
        /// </summary>
        internal static string GetExceptionStackDescription(this Exception ex)
        {
            StringBuilder errorDescription = new StringBuilder();

            Exception currentException = ex;
            while (currentException != null)
            {
                errorDescription.AppendLine("-- " + currentException.Message);
                currentException = currentException.InnerException;
            }

            return errorDescription.ToString();
        }
    }
}
