using GHVRQ_Save_Manager.Components;
using GHVRQ_Save_Manager.Data;

namespace GHVRQ_Save_Manager.Selectors;
using ConsumableComponent = Data.ConsumableComponent;

public class ComponentTemplateSelector : DataTemplateSelector
{
    public DataTemplate DamagableTemplate { get; set; }
    public DataTemplate DrinkableTemplate { get; set; }
    public DataTemplate ConsumableTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            DamagableComponent => DamagableTemplate,
            DrinkableComponent => DrinkableTemplate,
            ConsumableComponent => ConsumableTemplate,
            _ => DamagableTemplate
        };
    }
}