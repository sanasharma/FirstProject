using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.SysParams
{
    public class SysParamsListViewModel : Base.BaseViewModel
    {
        public QueryParameter Parameters { set; get; }
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
        public Pages Pages { set; get; }
    }

    public class ListItem
    {
        public string ParaCode { set; get; }
        public string ParaValue { set; get; }
        public string ParaDesc { set; get; }
    }

    public class QueryParameter
    {
        public string ParaCode { set; get; }
        public string ParaDesc { set; get; }
    }
}