using Microsoft.Extensions.Configuration;
using Rabbit.Extensions.Configuration.Internal;
using System;

namespace Rabbit.Extensions.Configuration
{
    public static class TemplateSupportExtensions
    {
        /// <summary>
        /// enable configuration template support.
        /// </summary>
        /// <param name="target">need enable configuration.</param>
        /// <param name="configure">options configure.</param>
        /// <returns>configuration.</returns>
        public static T EnableTemplateSupport<T>(this T target, Action<TemplateSupportOptions> configure = null)
            where T : IConfiguration
        {
            var options = new TemplateSupportOptions
            {
                Target = target,
                Source = target
            };

            configure?.Invoke(options);

            EnableTemplateSupport(options);

            return target;
        }

        #region Private Method

        /// <summary>
        /// enable configuration template support.
        /// </summary>
        private static void EnableTemplateSupport(TemplateSupportOptions options)
        {
            if (options.Source == null)
                throw new ArgumentNullException(nameof(options.Source));
            if (options.Source == null)
                throw new ArgumentNullException(nameof(options.Target));

            var target = options.Target;

            //register rerender listen
            if (options.RerenderOnChange)
                target.GetReloadToken().RegisterChangeCallback(s =>
                {
                    EnableTemplateSupport(options);
                }, null);

            var render = new TemplateRender(options);
            render.Render();
        }

        #endregion Private Method
    }
}