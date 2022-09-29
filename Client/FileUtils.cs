using Microsoft.JSInterop;

namespace AccReporting.Client
{
    public static class FileUtils
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
       => js.InvokeAsync<object>(
           "saveAsFile",
           filename,
           Convert.ToBase64String(data));

        public static ValueTask ShowReportInNewPage(this IJSRuntime js, byte[] data)
       => js.InvokeVoidAsync(
           "ShowReportNewPage",
           data);

        public static ValueTask<object> SaveAspdf(this IJSRuntime js, string filename, byte[] data)
        {
            var dt = Convert.ToBase64String(data);
            return js.InvokeAsync<object>(
           "jsSaveAsFile",
           filename,
           dt);
        }
    }
}