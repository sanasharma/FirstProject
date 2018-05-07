using System;
using System.Collections.Generic;
using Models.Model;

namespace Models.ViewModel.SysParams
{
    public class SysParamsDetailViewModel : Base.BaseViewModel
    {
        public string ParaCode { set; get; }
        public string ParaValue { set; get; }
        public string ParaDesc { set; get; }

        //頁面動作 Add, Edit
        public string Action { set; get; }
    }
}