using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Wokhan.UWP.UI.Xaml.Controls
{
    public sealed partial class ItemsControlGrid : ItemsControl
    {
        public ItemsControlGrid() : base()
        {
        }

     /*   public ColumnDefinitionCollection ColumnDefinitions
        {
            get;
        }

        public RowDefinitionCollection RowDefinitions
        {
            get;
        }
        */
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var r = base.IsItemItsOwnContainerOverride(item);
            return r;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (ItemTemplate == null)
            {
                return base.GetContainerForItemOverride();
            }

            var x = (FrameworkElement)ItemTemplate.LoadContent();
            return x;
        }

        public static ItemsControlGrid FromItemsControl(Grid parent, ItemsControl itms)
        {
            var icg = new ItemsControlGrid
            {
                ItemsPanel = itms.ItemsPanel,
                ItemTemplate = itms.ItemTemplate
            };
            parent.Children.Remove(itms);
            parent.Children.Add(icg);

            return icg;
        }
    }
}
