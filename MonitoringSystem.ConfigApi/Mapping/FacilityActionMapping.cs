using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Mapping; 

public static class FacilityActionMapping {
    public static FacilityActionDto ToDto(this FacilityAction action) {
        return new FacilityActionDto() {
            Id=action.Id,
            Name=action.Name,
            EmailEnabled = action.EmailEnabled,
            EmailPeriod = action.EmailPeriod,
            ActionType=action.ActionType
        };
    }

    public static FacilityAction ToEntity(this FacilityActionDto action) {
        return new FacilityAction() {
            Id=action.Id,
            Name=action.Name,
            EmailEnabled = action.EmailEnabled,
            EmailPeriod = action.EmailPeriod,
            ActionType=action.ActionType
        };
    }
    
    public static DeviceActionDto ToDto(this DeviceAction action) {
        var dto= new DeviceActionDto() {
            Id=action.Id,
            Name=action.Name,
            FirmwareId = action.FirmwareId,
            FacilityActionId = action.FacilityActionId,
            MonitorBoxId=action.MonitorBoxId,
            ActionType=action.FacilityAction.ActionType
        };
        return dto;
    }

    public static DeviceAction ToEntity(this DeviceActionDto action) {
        return new DeviceAction() {
            Id=action.Id,
            Name=action.Name,
            FirmwareId = action.FirmwareId,
            FacilityActionId = action.FacilityActionId,
            MonitorBoxId=action.MonitorBoxId
        };
    }
    
    public static ActionOutputDto ToDto(this ActionOutput actionOutput) {
        return new ActionOutputDto() {
            Id=actionOutput.Id,
            OnLevel=actionOutput.OnLevel,
            OffLevel=actionOutput.OnLevel,
            DeviceActionId = actionOutput.DeviceActionId,
            DiscreteOutputId=actionOutput.Id
        };
    }

    public static ActionOutput ToEntity(this ActionOutputDto actionOutput) {
        return new ActionOutput() {
            Id=actionOutput.Id,
            OnLevel=actionOutput.OnLevel,
            OffLevel=actionOutput.OnLevel,
            DeviceActionId = actionOutput.DeviceActionId,
            DiscreteOutputId=actionOutput.Id
        };
    }
}