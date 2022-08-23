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
    
    public static DeviceActionDto ToDto(this DeviceAction action) {
        var dto= new DeviceActionDto() {
            Id=action.Id,
            Name=action.Name,
            FirmwareId = action.FirmwareId,
            ActionOutputs = action.ActionOutputs.Select(e=>e.ToDto())
        };
        if (action.FacilityAction is not null) {
            dto.FacilityAction = action.FacilityAction.ToDto();
        }

        return dto;
    }
    
    public static ActionOutputDto ToDto(this ActionOutput actionOutput) {
        var dto= new ActionOutputDto() {
            Id=actionOutput.Id,
            OnLevel=actionOutput.OnLevel,
            OffLevel=actionOutput.OnLevel
        };
        if (actionOutput.DiscreteOutput is not null) {
            dto.DiscreteOutput = actionOutput.DiscreteOutput.ToDto();
        }

        return dto;
    }
}