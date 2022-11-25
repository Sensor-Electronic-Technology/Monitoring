namespace MonitoringWeb.WebApp.Services; 

public class ValueChanged<T> {
    public T? Item { get; private set; }

    public event Func<Task>? OnChildChanged;

    public event Func<Task>? OnParentChanged;
    
    public void SetItemChild(T item) {
        this.Item = item;
        this.NotifyChildChanged();
    }

    public void SetItemParent(T item) {
        this.Item = item;
        this.NotifyParentChanged();
    }

    private void NotifyParentChanged() {
        this.OnParentChanged?.Invoke();
    }
    
    private void NotifyChildChanged() {
        this.OnChildChanged?.Invoke();
    }
}