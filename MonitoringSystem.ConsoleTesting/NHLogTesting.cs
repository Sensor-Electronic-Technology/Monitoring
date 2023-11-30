﻿using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ConsoleTables;
using MailKit.Net.Smtp;
using MimeKit;
using Modbus.Device;
using MongoDB.Bson;
using MonitoringConfig.Data.Model;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.ConfigApi.Mapping;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Services;
using MonitoringSystem.Shared.Data.SettingsModel;
using ModbusDevice = MonitoringConfig.Data.Model.ModbusDevice;

namespace MonitoringSystem.ConsoleTesting; 

public class NHLogTesting {
    
    static async Task Main(string[] args) {
        //await CreateNHLogDatabase();
        //await AggregateTest();
        await CreateCalibrations();
    }

    static async Task CreateCalibrations() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_logs");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");
        
        var scale1 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 1,
            TankScaleState = TankScaleState.InUse
        };
        var scale2 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 2,
            TankScaleState = TankScaleState.IdleOnScaleMeasured
        };
        var scale3 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 3,
            TankScaleState = TankScaleState.NoTank
        };
        var scale4 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 4,
            TankScaleState = TankScaleState.IdleOnScaleNotMeasured
        };
        var scale1Calibration = new Calibration() {
            CalibrationDate = DateTime.Now.AddDays(-5),
            ZeroValue = 0,
            NonZeroValue = 100,
            ZeroRawValue = 576620,
            NonZeroRawValue = 658736,
            IsCurrent = true
        };
        var scale2Calibration = new Calibration() {
            CalibrationDate = DateTime.Now.AddDays(-5),
            ZeroValue = 0,
            NonZeroValue = 100,
            ZeroRawValue = 581616,
            NonZeroRawValue = 663909,
            IsCurrent = true
        };
        var scale3Calibration = new Calibration() {
            CalibrationDate = DateTime.Now,
            ZeroRawValue = 588465,
            NonZeroRawValue = 610038,
            ZeroValue = 0,
            NonZeroValue = 30,
            IsCurrent = true
        };

        var scale4Calibration = new Calibration() {
            CalibrationDate = DateTime.Now,
            ZeroRawValue = 1069977,
            NonZeroRawValue = 1113364,
            ZeroValue = 0,
            NonZeroValue = 26,
            IsCurrent = true
        };
        
        scale1.CurrentCalibration = scale1Calibration;
        scale2.CurrentCalibration = scale2Calibration;
        scale3.CurrentCalibration = scale3Calibration;
        scale4.CurrentCalibration = scale4Calibration;
        
        scale1.CalibrationLog.Add(scale1Calibration);
        scale2.CalibrationLog.Add(scale2Calibration);
        scale3.CalibrationLog.Add(scale3Calibration);
        scale4.CalibrationLog.Add(scale4Calibration);
        
        var tank1 = new NH3Tank() {
            StartDate = new DateTime(2023,8,1),
            SerialNumber = "1234s",
            StartWeight = 883,
        };
        
        var tank2 = new NH3Tank() {
            StartDate = new DateTime(2023,10,1),
            SerialNumber = "0923s",
            StartWeight = 884
        };
        
        var tank4 = new NH3Tank() {
            StartDate = new DateTime(2023,10,1),
            SerialNumber = "0923s",
            StartWeight = 884,
        };
        
        var tank1Measured = new TankWeight() {
            Gas = 883,
            Gross = 2614,
            Tare = 1732
        };
        
        var tank2Measured = new TankWeight() {
            Gas = 884,
            Gross = 2625,
            Tare = 1741
        };
        
        tank1.MeasuredWeight = tank1Measured;
        tank1.LabeledWeight = tank1Measured;
        
        tank2.MeasuredWeight = tank2Measured;
        tank2.LabeledWeight = tank2Measured;
        
        scale1.CurrentTank = tank1;
        scale2.CurrentTank = tank2;
        scale4.CurrentTank = tank4;
        
        await scaleCollection.InsertOneAsync(scale1);
        await scaleCollection.InsertOneAsync(scale2);
        await scaleCollection.InsertOneAsync(scale3);
        await scaleCollection.InsertOneAsync(scale4);
        Console.WriteLine("Check Database");
    }

    static async Task AggregateTest() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_tank_logs");
        var tankCollection = database.GetCollection<NH3Tank>("nh3_tanks");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");
        
        
        //scaleCollection.Aggregate()
        //scaleCollection.Aggregate()
        /*var stage1 =
            new BsonDocument("$lookup",
            new BsonDocument {
                { "from", "tank_scales" },
                { "localField", "TankScaleId" },
                { "foreignField", "_id" },
                { "as", "scale" }
            });
        var stage2 =
            new BsonDocument("$unwind",
            new BsonDocument("path", "$scale"));
        

        var results=tankCollection.Aggregate().AppendStage<BsonDocument>(stage1)
            .AppendStage<BsonDocument>(stage2).ToEnumerable();
        foreach (var result in results) {
            Console.WriteLine(result);
        }*/
    }

    static async Task CreateNHLogDatabase() {
        Console.WriteLine("Create NH Database");
        var client = new MongoClient("mongodb://172.20.3.41");
        var database=client.GetDatabase("nh3_tank_logs");
        var tankCollection = database.GetCollection<NH3Tank>("nh3_tanks");
        var scaleCollection = database.GetCollection<TankScale>("tank_scales");
        var scale1 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 1,
        };
        var scale2 = new TankScale() {
            _id = ObjectId.GenerateNewId(),
            ScaleId = 1,
        };
        var tank1 = new NH3Tank() {


            StartDate = new DateTime(2023,8,1),
            StopDate = new DateTime(2023,10,1),

            SerialNumber = "1234s",
            StartWeight = 883,
            StopWeight = 85
        };
        var tank2 = new NH3Tank() {


            StartDate = new DateTime(2023,10,1),

            SerialNumber = "0923s",
            StartWeight = 883
        };

        await scaleCollection.InsertOneAsync(scale1);
        await scaleCollection.InsertOneAsync(scale2);
        
        await tankCollection.InsertOneAsync(tank1);
        await tankCollection.InsertOneAsync(tank2);
        Console.WriteLine("Check Database");

    }
    
}
















