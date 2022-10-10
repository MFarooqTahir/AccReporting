using Microsoft.JSInterop;

namespace AccReporting.Client
{
    public static class FileUtils
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
       => js.InvokeAsync<object>(
           identifier: "saveAsFile",
           filename,
           Convert.ToBase64String(inArray: data));

        public static ValueTask ShowReportInNewPage(this IJSRuntime js, byte[] data)
       => js.InvokeVoidAsync(
           identifier: "ShowReportNewPage",
           data);

        public static ValueTask<object> SaveAspdf(this IJSRuntime js, string filename, byte[] data)
        {
            var dt = Convert.ToBase64String(inArray: data);
            return js.InvokeAsync<object>(
           identifier: "jsSaveAsFile",
           filename,
           dt);
        }
    }
}