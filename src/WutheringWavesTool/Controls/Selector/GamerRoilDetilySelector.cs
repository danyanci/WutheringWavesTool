using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.Controls.Selector;

public partial class GamerRoilDetilySelector : DataTemplateSelector
{
    public DataTemplate TypeItemTemplate { get; set; }

    public DataTemplate RoleItemTempalte { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is NavigationRoilsDetilyItem)
        {
            return RoleItemTempalte;
        }
        else
        {
            return TypeItemTemplate;
        }
        return base.SelectTemplateCore(item);
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is NavigationRoilsDetilyItem)
        {
            return RoleItemTempalte;
        }
        else
        {
            return TypeItemTemplate;
        }
        return base.SelectTemplateCore(item, container);
    }
}
