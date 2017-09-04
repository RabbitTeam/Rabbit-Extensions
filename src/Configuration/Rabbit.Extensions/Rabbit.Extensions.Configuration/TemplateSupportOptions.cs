namespace Rabbit.Extensions.Configuration
{
    public class TemplateSupportOptions : TemplateRenderOptions
    {
        /// <summary>
        /// render the child configuration.
        /// </summary>
        public bool EnableChildren { get; set; } = true;

        /// <summary>
        /// configure source changes to render again.
        /// </summary>
        public bool RerenderOnChange { get; set; } = true;
    }
}