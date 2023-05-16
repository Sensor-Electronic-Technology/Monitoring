namespace MonitoringWeb.WebApp.Services; 

public static class ColorExtensions {
    private static String HexConverter(System.Drawing.Color c) {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }

    private static String RGBConverter(System.Drawing.Color c) {
        return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
    }
}