using System;
using Newtonsoft.Json;

namespace Models.Base
{
    public class Pages
    {
        public int PageSize { set; get; }
        public int TotalRows { set; get; }
        public int PageIndex { set; get; }

        public Pages()
        {
            this.PageIndex = 1;
            this.PageSize = 10;
            this.TotalRows = 0;
        }

        public int PageCount
        {
            get
            {
                int c = TotalRows / PageSize;
                decimal d = TotalRows % PageSize;

                if (c <= 0)
                    return 1;
                else
                {
                    if (d > 0)
                    {
                        return c + 1;
                    }
                    else
                    {
                        return c;
                    }
                }
            }
        }
    }
}