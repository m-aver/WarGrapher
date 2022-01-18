using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WarGrapher.ViewModels;

namespace WarGrapher.Views
{
    /// <summary>
    /// Interaction logic for ItemSelectWindowView.xaml
    /// </summary>
    public partial class ItemSelectionWindowView : WindowViewBase
    {
        public ItemSelectionWindowView()
        {
            InitializeComponent();            
        }

        protected override bool ValidateViewModel(WindowViewModel viewModel) => viewModel is ItemSelectionWindowViewModel<EquipItemViewModel>;
    }
}
