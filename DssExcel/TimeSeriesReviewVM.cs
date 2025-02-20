﻿using Hec.Dss;
using Hec.Excel;
using SpreadsheetGear;
using System;

namespace DssExcel
{
  internal class TimeSeriesReviewVM : ValidationVM
  {
    private IWorksheet worksheet1 = null;
    string dssFileName1 = "";
    public TimeSeriesReviewVM(IWorksheet worksheet, string dssFileName)
    {
      this.worksheet1 = worksheet;
      dssFileName1 = dssFileName;
    }
    public override bool Validate(out string errorMessage)
    {
      using (DssWriter writer = new DssWriter(dssFileName1))
      {
        errorMessage = "";
        try
        {
          worksheet1.WorkbookSet.GetLock();
          TimeSeries[] tsList = ExcelTimeSeries.Read(worksheet1);
          // write to DSS

          foreach (var ts in tsList)
          {
            writer.Write(ts);
          }

        }
        catch (Exception e)
        {
          errorMessage = e.Message;
          return false;
        }
        finally
        {
          worksheet1.WorkbookSet.ReleaseLock();
        }
      }

      return true;
    }
  }
}
