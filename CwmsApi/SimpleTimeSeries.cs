﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CwmsData.Api
{
  internal class SimpleTimeSeries
  {
    public string Name { get; set; }
    public string Units { get; set; }

    public List<(DateTime Timestamp, double Value)> Points = new List<(DateTime, double)>();
    public Dictionary<string, string> Attributes = new();

    public void WriteToConsole()
    {
      foreach (var p in Points)
      {
        Console.WriteLine(p.Timestamp.ToString() + "," + p.Value);
      }
    }
    private static long ToUnixEpoch(DateTime dateTime)
    {
      return (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public string ToJson(string officeID )
    {
      List<List<object>> jsonList = Points.ConvertAll(point => new List<object> { ToUnixEpoch(point.Timestamp), point.Value, 0 });
      string jsonTS = JsonSerializer.Serialize(jsonList);

      string s = $@"
    {{
      ""name"": ""{Name}"",
      ""office-id"": ""{officeID}"",
      ""units"": ""{Units}"",
      ""values"": {jsonTS} 
    }}";
      return s;
    }

  }

}

