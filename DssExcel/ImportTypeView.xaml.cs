﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DssExcel
{
  /// <summary>
  /// Interaction logic for SelectDataType.xaml
  /// </summary>
  public partial class ImportTypeView : UserControl
  {
    public ImportTypeView(ImportTypeVM vm)
    {
      InitializeComponent();
      DataContext = vm;

      //ImportTypesList.SelectedIndex=0;
    }
  }
}
