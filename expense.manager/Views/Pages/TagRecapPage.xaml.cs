using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace expense.manager.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TagRecapPage : ContentPage
    {
        public TagRecapPage()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var swipeViewSent = (sender as SwipeView);

            if (swipeViewSent.Parent is CollectionView collectionView)
            {
                collectionView.SelectedItem = swipeViewSent.BindingContext;
            }

        }
    }
}