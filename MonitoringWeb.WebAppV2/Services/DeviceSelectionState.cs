using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringWeb.WebAppV2.Services; 

public class DeviceSelectionState {
    public ModbusDeviceDto? SelectedDevice { get; private set; }
    public event Action? OnChange;

    public void SetDevice(ModbusDeviceDto device) {
        this.SelectedDevice = device;
        this.NotifyStateChanged();
    }
    
    private void NotifyStateChanged() {
        this.OnChange?.Invoke();   
    }
}

public class SelectionChanged<T> {
    public T? SelectedItem { get; private set; }

    public event Action? OnChanged;

    public void SetItem(T item) {
        this.SelectedItem = item;
        this.NotifySelectionChanged();
    }

    private void NotifySelectionChanged() {
        this.OnChanged?.Invoke();
    }
}
