using Rabbit.Extensions.Configuration.Utilities;
using System;
using System.Text;

namespace Rabbit.Extensions.Configuration.Internal
{
    public class TemplateEntry
    {
        public TemplateEntry(string key, string template)
        {
            Key = key;
            Template = template;
        }

        public string Key { get; }
        private string _template;

        public string Template
        {
            get => _template;
            set
            {
                _template = value;
                Builder = new StringBuilder(_template);
                Variables = TemplateUtil.GetVariables(value);
            }
        }

        public StringBuilder Builder { get; private set; }
        private string _value;

        public string Value
        {
            get => _value;
            private set
            {
                _value = value;
                Rendered = true;
            }
        }

        public bool Rendered { get; private set; }
        public string[] Variables { get; set; }

        public void Render(string[] variables, TemplateRenderOptions options)
        {
            if (Rendered)
                return;

            var target = options.Target;
            var source = options.Source;

            if (target == null)
                throw new ArgumentNullException(nameof(options.Target));
            if (source == null)
                throw new ArgumentNullException(nameof(options.Source));

            void Replace(string key)
            {
                var replaceText = TemplateUtil.GetReplaceText(key);
                var value = source[key];
                if (value == null)
                {
                    switch (options.VariableMissingAction)
                    {
                        case VariableMissingAction.UseKey:
                            value = replaceText;
                            break;

                        case VariableMissingAction.UseEmpty:
                            value = null;
                            break;

                        case VariableMissingAction.ThrowException:
                            throw new ArgumentException($"missing key '{key}'.");
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                Builder.Replace(replaceText, value);
            }

            foreach (var variable in variables)
            {
                Replace(variable);
            }

            Value = TemplateUtil.Escaped(Builder.ToString());
            target[Key] = Value;
        }
    }
}