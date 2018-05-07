using System;
using System.Collections.Generic;
using Models.Model;

namespace Models.ViewModel.TagRule
{
    public class TagRuleDetailViewModel : Base.BaseViewModel
    {
        public string Action { get; set; }
        public string FilterCode { get; set; }
        public string FilterName { get; set; }
        public string FilterRules { get; set; }
    }
}