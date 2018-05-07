using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.TagRule
{
    public class TagRuleListViewModel : Base.BaseViewModel
    {
        public List<Filter> List { set; get; }
        public Auth Authority { set; get; }
    }
}