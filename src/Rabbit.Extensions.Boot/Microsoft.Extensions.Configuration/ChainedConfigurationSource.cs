using System;
using System.Collections.Generic;
using System.Text;
namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Represents a chained IConfiguration as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class ChainedConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The chained configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Builds the <see cref="ChainedConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ChainedConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new ChainedConfigurationProvider(this);
    }
}