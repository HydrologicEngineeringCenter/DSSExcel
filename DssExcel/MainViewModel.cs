﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DssExcel
{
  public enum ImportType
  {
    TimeSeries,
    PairedData,
  }


  public class MainViewModel : BaseVM
  {
    public ImportTypeVM ImportTypeVM;

    public MainViewModel()
    {
      ImportTypeVM = new ImportTypeVM();
    }
    public string ExcelFileName { get; set; }

    public ImportType ImportType
    {
      get { return ImportTypeVM.SelectedImportType.Type;}
    }

    public DateTime[] DateTimes { get; set; } 

    public double[] XValues { get; set; }
    public double[,] YValues { get; set; }
    public double[,] TimeSeriesValues { get; set; }
    public string[] TimeSeriesNames { get; set; }

    public string DssFileName { get; set; }

    
    internal ExcelReader ExcelReader { get; set; }
  }


}
