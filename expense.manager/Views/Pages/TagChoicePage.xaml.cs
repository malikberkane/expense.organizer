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
    public partial class TagChoicePage
    {
        public TagChoicePage()
        {
            InitializeComponent();

            CollectionView.SelectionChanged += this.CollectionView_SelectionChanged;

        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var a = e.CurrentSelection;
            var b = e.PreviousSelection;
        }
    }
}