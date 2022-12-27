﻿using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hec.Dss;

namespace DssExcel
{
  /// <summary>
  /// ExcelTimeSeries has methods for reading and writing time series data to excel using the 
  /// format below.  Multiple series are supported by adding additional colums D, E, etc.
  /// The row labeled D is intentionally skipped
  /// 
  /// +-------+-----------------+-------------+
  /// |   A   |        B        |      C      |
  /// +-------+-----------------+-------------+
  /// | A     |  watershed      | CARUTHERS C |
  /// | B     |  location       | IVANPAH CA  |
  /// | C     |  parameter      | FLOW        |
  /// | E     |  interval/block |             |
  /// | F     |  version/tag    | USGS        |
  /// | Units |                 | CFS         |
  /// | Type  |                 | INST-VAL    |
  /// | 1     | 31May2020  2300 | 0.0         |
  /// | 2     | 31May2020  2315 | 0.0         |
  /// | 3     | 31May2020  2330 | 0.0         |
  /// | 4     | 31May2020  2345 | 0.0         |
  /// | 5     | 01Jun2020  0000 | 0.0         |
  /// | 6     | 01Jun2020  0015 | 0.0         |
  /// | 7     | 01Jun2020  0030 | 0.0         |
  /// +-------+-----------------+-------------+
  /// 
  /// 
  /// </summary>
  public class ExcelTimeSeries
  {

    private static string[] firstColumn = { "A", "B", "C", "E", "F", "Units", "Type" };
    private static (int r, int c) indexOfWatershed = (0, 2);
    private static (int r, int c) indexOfLocation = (1, 2);
    private static (int r, int c) indexOfParameter = (2, 2);
    private static (int r, int c) indexOfInterval = (3, 2);
    private static (int r, int c) indexOfVersion = (4, 2);
    private static (int r, int c) indexOfUnits = (5, 2);
    private static (int r, int c) indexOfType = (6, 2);
    private static (int r, int c) indexDates = ( 7, 1);
    private static (int r, int c) indexValues = ( 7, 2);



    public static void Write(IWorksheet worksheet, TimeSeries[] series)
    {
      if( series.Length == 0 )
        throw new Exception("There are no series to write to excel.");

      worksheet.WorkbookSet.GetLock();
      try
      {
        var range = worksheet.Cells;
        range.Clear();
        Excel.WriteArrayDown(range, firstColumn);
        var ts = series[0];
        string ePart = "";
        if (ts.IsRegular(ts.Times))
          ePart = "interval:";
        else
          ePart = "block-size:";
        Excel.WriteArrayDown(range[0, 1], new string[] { "group:", "location:", "parameter:", ePart, "version:", "units (cfs,feet,...):", "  type(PER-AVER,PER-CUM,INST-VAL,INST-CUM):" });
        Excel.WriteSequenceDown(range[indexDates.r, 0], 1, ts.Times.Length);
        Excel.WriteArrayDown(range[indexDates.r, indexDates.c], ts.Times);

        for (int i = 0; i < series.Length; i++)
        {
          ts = series[i];
          range[indexOfWatershed.r, indexOfWatershed.c + i].Value = ts.Path.Apart;
          range[indexOfLocation.r, indexOfLocation.c + i].Value = ts.Path.Bpart;
          range[indexOfParameter.r, indexOfParameter.c + i].Value = ts.Path.Cpart;
          range[indexOfInterval.r, indexOfInterval.c + i].Value = TimeWindow.GetInterval(ts);
          range[indexOfVersion.r, indexOfVersion.c + i].Value = ts.Path.Fpart;
          range[indexOfUnits.r, indexOfUnits.c + i].Value = ts.Units;
          range[indexOfType.r, indexOfType.c + i].Value = ts.DataType;

          Excel.WriteArrayDown(range[indexValues.r, indexValues.c + i], ts.Values);
        }
        
        worksheet.Cells["A:A"].Columns.AutoFit();
        worksheet.Cells["B:B"].Columns.AutoFit();
      }
      finally
      {
        worksheet.WorkbookSet.ReleaseLock();
      }

    }

    public static TimeSeries[] Read(string excelFileName, string sheetName = "sheet1")
    {
      var workbook = SpreadsheetGear.Factory.GetWorkbook(excelFileName);
      var sheet = workbook.Worksheets[sheetName];
      TimeSeries[] tsList = Read(sheet);

      return tsList;
    }

    public static TimeSeries[] Read(IWorksheet worksheet)
    {
      var cells =worksheet.Range;
      if (!Excel.IsMatchDown(cells, firstColumn))
        return null;

      var usedRange = worksheet.GetUsedRange(true);

      var dateCells = worksheet.Cells[indexDates.r, indexDates.c,usedRange.RowCount-1, indexDates.c];

      if(! Excel.TryGetDateArray(dateCells,out DateTime[] dates,out string errorMessage))
      {
        Logging.WriteError(errorMessage);
        throw new Exception(errorMessage);
      }
      // find how many series by reading first value for each series.
      string[] firstRow = Excel.ReadStringsAcross(worksheet, cells[indexValues.r, indexValues.c],true);


      var rval = new List<TimeSeries>(firstRow.Length);
      for (int i = 0; i < firstRow.Length; i++)
      {
        var valueCells = worksheet.Cells[indexValues.r, indexValues.c+i, usedRange.RowCount - 1, indexValues.c+i];
        if(!Excel.TryGetValueArray(valueCells, out double[] values, out errorMessage))
        {
          Logging.WriteError(errorMessage);
        }
        TimeSeries ts = new TimeSeries();
        ts.Times = dates;
        ts.Values = values;
        ts.Units = Excel.CellString(cells[indexOfUnits.r, indexOfUnits.c + i]);
        ts.DataType = Excel.CellString(cells[indexOfType.r, indexOfType.c + i]);
        ts.Path = GetDssPath(cells[indexOfWatershed.r, indexOfWatershed.c + i]);
        rval.Add(ts);
      }


      return rval.ToArray();
    }

    private static DssPath GetDssPath(IRange range)
    {
      var a = Excel.CellString(range[0, 0]);
      var b = Excel.CellString(range[1, 0]);
      var c = Excel.CellString(range[2, 0]);
      var d = "";
      var e = Excel.CellString(range[3, 0]);
      var f = Excel.CellString(range[4, 0]);
      DssPath p = new DssPath(a, b, c, d, e, f);

      return p;
    }
  }
}

