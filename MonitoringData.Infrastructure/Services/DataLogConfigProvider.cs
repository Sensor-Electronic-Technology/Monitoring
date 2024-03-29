﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;

namespace MonitoringData.Infrastructure.Services; 

public class DataLogConfigProvider:IMonitorConfigurationProvider {
    private readonly IMongoClient _client;
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private IMongoCollection<EmailRecipient> _emailRecipientCollection;
    private IMongoCollection<BulkEmailSettings> _bulkEmailSettingsCollection;
    private readonly MonitorDataLogSettings _settings;
    private readonly MonitorEmailSettings _emailSettings;
    private ManagedDevice _device;
    private BulkEmailSettings _bulkEmailSettings;
    private List<EmailRecipient> _emailRecipients;
    
    public MonitorEmailSettings MonitorEmailSettings => this._emailSettings;
    public MonitorDataLogSettings MonitorDataLogSettings => this._settings;
    public IEnumerable<EmailRecipient> EmailRecipients => this._emailRecipients.AsEnumerable();
    public BulkEmailSettings BulkEmailSettings => this._bulkEmailSettings;
    public ManagedDevice ManagedDevice => this._device;
    public string DeviceName { get; set; }
    
    public DataLogConfigProvider(IMongoClient client, 
        IOptions<MonitorDataLogSettings> settings,
        IOptions<MonitorEmailSettings> emailSettings) {
        this._client = client;
        this._settings = settings.Value;
        this._emailSettings = emailSettings.Value;
        var database = this._client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
        this._emailRecipientCollection = database.GetCollection<EmailRecipient>(this._settings.EmailRecipientCollection);
        this._bulkEmailSettingsCollection = database.GetCollection<BulkEmailSettings>(this._settings.BulkEmailSettings);
    }
    public DataLogConfigProvider(IMongoClient client,
        MonitorDataLogSettings settings,
        MonitorEmailSettings emailSettings) {
        this._client = client;
        this._settings = settings;
        this._emailSettings = emailSettings;
        var database = this._client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
        this._emailRecipientCollection = database.GetCollection<EmailRecipient>(this._settings.EmailRecipientCollection);
        this._bulkEmailSettingsCollection = database.GetCollection<BulkEmailSettings>(this._settings.BulkEmailSettings);
    }
    
    
    public async Task Load() {
        this._emailRecipients=await this._emailRecipientCollection.Find(_ => true).ToListAsync();
        this._bulkEmailSettings=await this._bulkEmailSettingsCollection.Find(_ => true)
            .FirstOrDefaultAsync();
        this._device = await this._deviceCollection.Find(e => e.DeviceName.ToLower() == this.DeviceName.ToLower())
            .FirstOrDefaultAsync();

    }
    
    public async Task Reload() {
        var database = this._client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
        this._emailRecipientCollection = database.GetCollection<EmailRecipient>(this._settings.EmailRecipientCollection);
        this._bulkEmailSettingsCollection = database.GetCollection<BulkEmailSettings>(this._settings.BulkEmailSettings);
        this._emailRecipients=await this._emailRecipientCollection.Find(_ => true).ToListAsync();
        this._device = await this._deviceCollection.Find(e => e.DeviceName.ToLower() == this.DeviceName.ToLower())
            .FirstOrDefaultAsync();
        this._bulkEmailSettings=await this._bulkEmailSettingsCollection.Find(_ => true)
            .FirstOrDefaultAsync();
    }
}