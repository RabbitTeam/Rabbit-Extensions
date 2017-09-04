using Microsoft.Extensions.Configuration;
using Rabbit.Extensions.Configuration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Extensions.Configuration.Internal
{
    public class TemplateRender
    {
        private readonly TemplateRenderOptions _options;
        private readonly IList<TemplateEntry> _renders = new List<TemplateEntry>();

        public TemplateRender(TemplateRenderOptions options)
        {
            _options = options;
            AddEntries(options.Target.GetChildren());
        }

        public void Render()
        {
            while (_renders.Any(i => !i.Rendered))
            {
                foreach (var render in _renders)
                {
                    CheckDependencies(render, new List<TemplateEntry>());

                    var variables = FindVariables(render.Variables);
                    if (variables == null || variables.All(i => i.Rendered))
                    {
                        render.Render(render.Variables, _options);
                    }
                }
            }
        }

        #region Private Method

        private void AddEntries(IEnumerable<IConfigurationSection> sections)
        {
            foreach (var section in sections)
            {
                var key = section.Path;
                var value = section.Value;

                if (value == null)
                {
                    AddEntries(section.GetChildren());
                }
                else if (TemplateUtil.NeedRender(value))
                {
                    AddEntry(key, value);
                }
            }
        }

        private void AddEntry(string key, string template)
        {
            var needRender = TemplateUtil.NeedRender(template);
            if (!needRender)
                return;

            var entry = new TemplateEntry(key, template);
            _renders.Add(entry);
        }

        private void CheckDependencies(TemplateEntry entry, ICollection<TemplateEntry> dependencies)
        {
            var variables = entry.Variables;
            if (variables == null || !variables.Any())
                return;

            foreach (var variable in variables)
            {
                var dependency = _renders.SingleOrDefault(i => i.Key == variable);
                if (dependency == null)
                    continue;

                if (dependencies.Any(i => i.Key == dependency.Key))
                    throw new ArgumentException($"cyclic dependency '{entry.Key} > {string.Join(" > ", dependencies.Select(i => i.Key))}'.", entry.Key);

                dependencies.Add(dependency);

                if (dependency.Variables != null && dependency.Variables.Any())
                {
                    CheckDependencies(dependency, dependencies);
                }
            }
        }

        private TemplateEntry[] FindVariables(string[] variables)
        {
            return variables == null ? null : _renders.Where(i => variables.Contains(i.Key)).ToArray();
        }

        #endregion Private Method
    }
}