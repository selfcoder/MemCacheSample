using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MemCache.WpfClient
{
    [ContentProperty("Template")]
    public class TypeTemplate
    {
        public Type Type { get; set; }
        public DataTemplate Template { get; set; }
    }

    public class TypeTemplateCollection : List<TypeTemplate>
    {
    }

    [ContentProperty("Templates")]
    public class TypeDataTemplateSelector : DataTemplateSelector
    {
        public TypeTemplateCollection Templates { get; private set; }

        public TypeDataTemplateSelector()
        {
            Templates = new TypeTemplateCollection();
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return SelectTemplate(item, container, (o, t) => t.IsInstanceOfType(o));
        }

        protected DataTemplate SelectTemplate(object item, DependencyObject container, Func<object, Type, bool> typeMatchPredicate)
        {
            TypeTemplate firstDefaultTemplate = null;
            foreach (TypeTemplate template in Templates)
            {
                Type templateDataType = template.Type;
                if (templateDataType == null && template.Template != null)
                {
                    templateDataType = template.Template.DataType as Type;
                }

                if (templateDataType != null)
                {
                    if (item != null && typeMatchPredicate(item, templateDataType))
                    {
                        return template.Template;
                    }

                    continue;
                }

                if (firstDefaultTemplate == null)
                {
                    firstDefaultTemplate = template;
                }
            }

            if (firstDefaultTemplate != null)
            {
                return firstDefaultTemplate.Template;
            }

            return null;
        }
    }
}