using Microsoft.Extensions.Configuration;

namespace Rabbit.Extensions.Configuration
{
    /// <summary>
    /// Can not find the replacement behavior of the variable.
    /// </summary>
    public enum VariableMissingAction
    {
        /// <summary>
        /// not replace.
        /// </summary>
        UseKey,

        /// <summary>
        /// replace empty.
        /// </summary>
        UseEmpty,

        /// <summary>
        /// throw exception.
        /// </summary>
        ThrowException
    }

    public class TemplateRenderOptions
    {
        /// <summary>
        /// Can not find the replacement behavior of the variable.
        /// </summary>
        public VariableMissingAction VariableMissingAction { get; set; } = VariableMissingAction.UseKey;

        /// <summary>
        /// handle target.
        /// </summary>
        public IConfiguration Target { get; set; }

        /// <summary>
        /// variable value source.
        /// </summary>
        public IConfiguration Source { get; set; }
    }
}