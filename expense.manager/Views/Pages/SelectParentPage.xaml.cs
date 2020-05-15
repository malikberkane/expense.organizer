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
    public partial class SelectParentPage : ContentPage
    {
        public SelectParentPage()
        {
            InitializeComponent();
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}