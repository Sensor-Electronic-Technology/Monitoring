﻿using MongoDB.Bson;

namespace MonitoringWeb.WebApp.Data;
public class AnalogReadingDto {
    public string Name { get; set; }
    public DateTime TimeStamp { get; set; }
    public double Time { get; set; }
    public double Value { get; set; }
}