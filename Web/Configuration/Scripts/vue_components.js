//分頁組件 (2017-03-01 By Jones)
Vue.component('Pagination', {
    props: ['value'],
    template: '\
            <label class="pull-right">\
                顯示筆數：\
                <select class="input-sm" v-model="value.PageSize" v-on:change="changePageSize">\
                    <option>10</option>\
                    <option>25</option>\
                    <option>50</option>\
                </select>　\
                頁數：\
                <div class="btn-group btn-overlap">\
                    <label class="btn btn-sm btn-white" :class="{ disabled: value.PageIndex == 1 }" v-on:click="changePageIndex(1)">\
                        <i class="icon-only ace-icon fa fa-angle-double-left bigger-110"></i>\
                    </label>\
                    <label class="btn btn-sm btn-white" :class="{ disabled: value.PageIndex == 1 }" v-on:click="changePageIndex(value.PageIndex - 1)">\
                        <i class="icon-only ace-icon fa fa-angle-left bigger-110"></i>\
                    </label>\
                    <label class="btn btn-sm btn-white" :class="{ disabled: value.PageIndex == value.PageCount }" v-on:click="changePageIndex(value.PageIndex + 1)">\
                        <i class="icon-only ace-icon fa fa-angle-right bigger-110"></i>\
                    </label>\
                    <label class="btn btn-sm btn-white" :class="{ disabled: value.PageIndex == value.PageCount }" v-on:click="changePageIndex(value.PageCount)">\
                        <i class="icon-only ace-icon fa fa-angle-double-right bigger-110"></i>\
                    </label>\
                    <input type="text" class="input-sm" style="width:50px;" v-model="value.PageIndex" v-on:change="changePageIndex(value.PageIndex)"></input>\
                </div>\
                / {{ value.PageCount }}　\
                總筆數：{{ value.TotalRows }}\
            </label>',
    methods: {
        changePageSize: function (e) {
            location.href = changeURLArg(location.href, 'PageSize', this.value.PageSize);
        },
        changePageIndex: function (index) {
            if (index < 1 | index > this.value.PageCount) index = 1;
            location.href = changeURLArg(location.href, 'PageIndex', index);
        }
    }
});

//訊息區組件 (2017-03-01 By Jones)
Vue.component('Alert', {
    props: ['value'],
    template: '\
            <div v-bind:class="type" v-if="value.Code != 0">\
                <button type="button" class="close" v-on:click="value.Code = 0">\
                    <i class="ace-icon fa fa-times"></i>\
                </button>\
                <strong>\
                    <i v-bind:class="icon"></i>\
                    {{ title }}\
                </strong>\
                {{ value.Msg }}\
            </div>\
        ',
    computed: {
        type: function () {
            switch (this.value.Code) {
                case 1: return 'alert alert-success';
                case 2: return 'alert alert-danger';
                case 3: return 'alert alert-info';
                case 4: return 'alert alert-warning';
                default: return '';
            }
        },
        icon: function () {
            switch (this.value.Code) {
                case 1: return 'ace-icon fa fa-check';
                case 2: return 'ace-icon fa fa-times';
                case 3: return 'ace-icon fa fa-info';
                case 4: return 'ace-icon fa fa-exclamation';
                default: return '';
            }
        },
        title: function () {
            switch (this.value.Code) {
                case 1: return 'Well done!';
                case 2: return 'Oh snap!';
                case 3: return 'Heads up!';
                case 4: return 'Warning!';
                default: return '';
            }
        }
    }
});
