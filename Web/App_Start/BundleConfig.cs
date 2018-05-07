using System.Web;
using System.Web.Optimization;

namespace EPC
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //樣式檔
            bundles.Add(new StyleBundle("~/Style").Include(
                "~/Configuration/Content/bootstrap.min.css",
                "~/Configuration/Content/bootstrap-select.min.css",
                "~/Configuration/Content/font-awesome.min.css",
                "~/Configuration/Content/fonts.css",
                "~/Configuration/Content/ace.min.css"));

            //js檔
            bundles.Add(new ScriptBundle("~/Scripts").Include(
                "~/Configuration/Scripts/ace-extra.min.js",
                "~/Configuration/Scripts/jquery-2.1.1.min.js",
                "~/Configuration/Scripts/jquery.validate.min.js",
                "~/Configuration/Scripts/bootstrap.min.js",
                "~/Configuration/Scripts/bootstrap-select.min.js",
                "~/Configuration/Scripts/ace-elements.min.js",
                "~/Configuration/Scripts/ace.min.js",
                "~/Configuration/Scripts/bootbox.min.js",
                "~/Configuration/Scripts/vue.min.js",
                "~/Configuration/Scripts/vue-resource.min.js",
                "~/Configuration/Scripts/vue_components.js",
                "~/Configuration/Scripts/FileSaver.min.js",
                "~/Configuration/Scripts/util.js"));
        }
    }
}
