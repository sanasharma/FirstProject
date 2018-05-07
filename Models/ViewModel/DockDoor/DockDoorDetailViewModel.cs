using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.DockDoor
{
    public class DockDoorDetailViewModel : Base.BaseViewModel
    {
        public int ID { set; get; }
        public string DockDoorID { set; get; }
        public string IP { set; get; }
        public string CaptionPanelIP { set; get; }
        public string Locate { set; get; }
        public string Remark { set; get; }
        public string FilterCode { set; get; }
        public string Truck_ID { set; get; }
        public string TRUCK_NO { set; get; }
        
        public bool Flag { set; get; }
        public int Mask { set; get; }
        public int MaskOut { set; get; }
        public DateTime CreateTime { set; get; }
        public DateTime UpdateTime { set; get; }

        public List<Filter> FilterList { set; get; }

        public QueryParameter Parameters { set; get; }

        public List<RFID_TRUCK> List { set; get; }

        public List<RFID_TRUCK_Filter> TruckFilterList { set; get; }

        public List<CompareList> compareList { get; set; }


        public Pages Pages { set; get; }

        public Auth Authority { set; get; }

        public class RFID_TRUCK
        {
            public string TRUCK_NO { set; get; }

            public string Truck_ID { set; get; }


          //  public string SelectedTruck { set; get; }
            
        }
        public class RFID_TRUCK_Filter
        {
            public string TRUCK_NO { set; get; }

        }



        public class DockDoorList
        {
            public string DockDoorID { set; get; }

            public string ContainerID { set; get; }


        }

        public class CompareList
        {
            public string ContainerID { set; get; }

            public bool Flag { get; set; }


        }

        public class QueryParameter
        {
            public string TRUCK_NO { set; get; }


        }
    }
}