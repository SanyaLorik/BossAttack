using SanyaBeerExtension;

public class CapacityVisualizer : ProgressVisualizer {
    
    public void SetDontShow() {
        if (_progressContainer.gameObject.activeSelf) {
            _progressContainer.DisactiveSelf();
        }
    }

    
    public void SetCapacityValue(int currentValue, int maxValue) {
        float percentage = (float)currentValue / maxValue;
        SetProgressPercentage(percentage, currentValue);
    }
}