﻿using Hec.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DssExcel
{
  internal class RangeSelectionTimeSeriesValues : RangeSelectionVM
  {
    public RangeSelectionTimeSeriesValues(MainViewModel vm) : base(vm)
    {
      Title = "Select time series values";
      Description = "select one or more ranges with numbers";
    }

    public override bool Validate(out string errorMessage)
    {

      mainViewModel.SecondRangeText="values: " +Excel.RangeToString(RangeSelection);
      if( !Excel.TryGetValueArray2D(RangeSelection, out double[,] values, out errorMessage, MainViewModel.MissingValue))
      {
        return false;
      }

      mainViewModel.TimeSeriesValues = values;
      mainViewModel.TimeSeriesNames = Excel.RangeTitles(RangeSelection);
      return true;

    }
  }
}
