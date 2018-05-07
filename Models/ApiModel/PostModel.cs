using System;
using System.Collections.Generic;

namespace Models.ApiModel
{
    public class PostModel
    {
        public string ip { set; get; }
        public List<PalletItem> pallets { set; get; }
        public List<BoxItem> boxs { set; get; }
    }

    public class PalletItem
    {
        public string pallet { set; get; }
        public string count { set; get; }
        public string rssi { set; get; }
    }

    public class BoxItem
    {
        public string box { set; get; }
        public string count { set; get; }
        public string rssi { set; get; }
    }
}